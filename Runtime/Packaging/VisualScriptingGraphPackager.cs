// Visual Pinball Engine
// Copyright (C) 2026 freezy and VPE Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace VisualPinball.Unity.VisualScripting
{
	/// <summary>
	/// Packages the Unity Visual Scripting graphs of a table into a <c>.vpe</c> package and restores
	/// them at runtime.
	///
	/// <para>
	/// Visual scripting drives the game logic through <c>ScriptMachine</c>/<c>StateMachine</c>
	/// components plus the <c>ScriptGraphAsset</c>/<c>StateGraphAsset</c> graph assets they reference.
	/// None of these are VPE components, so the glTF/<see cref="IPackable"/> pipeline drops them. This
	/// packager bridges that gap: it serializes every machine and the transitive closure of graph
	/// assets they reference into portable <c>{json, objectReferences}</c> blobs (Unity Visual
	/// Scripting's own <see cref="SerializationData"/>), remapping the object references to stable ids
	/// so the macro-to-macro links survive the round trip.
	/// </para>
	///
	/// <para>
	/// On import the graph assets are recreated with <see cref="ScriptableObject.CreateInstance(Type)"/>
	/// and the machines are re-added to their original nodes — exactly the data path Unity itself uses
	/// when it loads these objects (<c>_data.DeserializeInto(ref this, true)</c>). Because every macro
	/// instance exists before any graph is deserialized, the cross-graph dependencies resolve through
	/// Visual Scripting's normal dependency mechanism.
	/// </para>
	/// </summary>
	public static class VisualScriptingGraphPackager
	{
		/// <summary>
		/// Captures the visual scripting machines and their graph-asset closure under <paramref name="root"/>.
		/// </summary>
		/// <returns>The packed payload, or an empty array when there is nothing to pack.</returns>
		public static byte[] Pack(Transform root, PackagedRefs refs)
		{
			if (root == null) {
				return Array.Empty<byte>();
			}

			// Machines (ScriptMachine/StateMachine) drive the logic; Variables components carry
			// object-scoped variable declarations the graphs may read.
			var machines = root.GetComponentsInChildren<IMachine>(true).OfType<Component>().ToList();
			var variables = root.GetComponentsInChildren<Variables>(true).Cast<Component>().ToList();

			if (machines.Count == 0 && variables.Count == 0) {
				return Array.Empty<byte>();
			}

			// Scene-scoped variables live on a scene singleton that is created on demand by Visual
			// Scripting itself and is usually not part of the table hierarchy, so we cannot bind it to
			// a node. Surface it if it ever carries data so the gap is visible rather than silent.
			foreach (var sceneVariables in root.GetComponentsInChildren<SceneVariables>(true)) {
				Debug.LogWarning($"[VS] SceneVariables on '{sceneVariables.name}' is not packaged; " +
					"scene-scoped variables are not yet supported by .vpe packaging.");
			}

			var data = new VisualScriptingGraphData();
			var graphIdByInstanceId = new Dictionary<int, string>();
			var graphQueue = new Queue<ScriptableObject>();

			foreach (var comp in variables) {
				var behaviour = PackBehaviour(comp, refs, graphIdByInstanceId, graphQueue);
				if (behaviour != null) {
					data.Variables.Add(behaviour);
				}
			}
			foreach (var comp in machines) {
				var behaviour = PackBehaviour(comp, refs, graphIdByInstanceId, graphQueue);
				if (behaviour != null) {
					data.Machines.Add(behaviour);
				}
			}

			// Breadth-first walk over the macro closure. Each graph asset may reference further graph
			// assets (sub-graphs / nested states), which are discovered while mapping its references.
			while (graphQueue.Count > 0) {
				var asset = graphQueue.Dequeue();
				data.Graphs.Add(PackGraph(asset, graphIdByInstanceId, graphQueue));
			}

			Debug.Log($"[VS] Packed {data.Machines.Count} machine(s), {data.Variables.Count} variables " +
				$"component(s) and {data.Graphs.Count} graph asset(s).");

			return PackageApi.Packer.Pack(data);
		}

		/// <summary>
		/// Recreates the visual scripting machines and graph assets captured by <see cref="Pack"/>.
		/// Must run while <paramref name="root"/> is inactive so the machines do not instantiate their
		/// graphs before everything is wired.
		/// </summary>
		/// <returns>The graph assets created at runtime, so their lifetime can be tied to the table.</returns>
		public static List<ScriptableObject> Unpack(byte[] bytes, Transform root, PackagedRefs refs)
		{
			var created = new List<ScriptableObject>();
			if (bytes == null || bytes.Length == 0) {
				return created;
			}

			var data = PackageApi.Packer.Unpack<VisualScriptingGraphData>(bytes);
			if (data == null) {
				return created;
			}

			// Pass 1: create every graph-asset instance. Macro<TGraph>.OnEnable() runs synchronously
			// here and registers the asset as an available serialization dependency, so cross-graph
			// references resolve in pass 2 regardless of order.
			var instanceById = new Dictionary<string, ScriptableObject>();
			foreach (var graph in data.Graphs) {
				var type = VisualScriptingTypeResolver.Resolve(graph.TypeName);
				if (type == null) {
					Debug.LogError($"[VS] Cannot resolve graph asset type '{graph.TypeName}'; skipping.");
					continue;
				}
				var asset = ScriptableObject.CreateInstance(type);
				asset.name = string.IsNullOrEmpty(graph.Name) ? type.Name : graph.Name;
				// Runtime-only asset; keep it out of any scene/save and let the table own its lifetime.
				asset.hideFlags = HideFlags.HideAndDontSave;
				instanceById[graph.Id] = asset;
				created.Add(asset);
			}

			// Pass 2: deserialize the graph content into each asset, rebuilding its object-reference
			// array from the recreated instances.
			foreach (var graph in data.Graphs) {
				if (!instanceById.TryGetValue(graph.Id, out var asset)) {
					continue;
				}
				RestoreInto(asset, graph.Json, graph.ObjectRefs, instanceById, $"graph '{graph.Name}'");
			}

			// Pass 3: re-add the components to their nodes. Variables first so a machine that reads
			// object variables finds its declarations in place.
			foreach (var behaviour in data.Variables) {
				RestoreBehaviour(behaviour, refs, instanceById);
			}
			foreach (var behaviour in data.Machines) {
				RestoreBehaviour(behaviour, refs, instanceById);
			}

			Debug.Log($"[VS] Restored {data.Machines.Count} machine(s), {data.Variables.Count} variables " +
				$"component(s) and {data.Graphs.Count} graph asset(s).");

			return created;
		}

		#region Pack helpers

		private static VisualScriptingBehaviourData PackBehaviour(Component comp, PackagedRefs refs,
			Dictionary<int, string> graphIds, Queue<ScriptableObject> graphQueue)
		{
			var nodeId = refs.GetNodeId(comp.transform);
			if (nodeId == null) {
				Debug.LogWarning($"[VS] {comp.GetType().Name} on '{comp.name}' is on a non-exported node " +
					"and cannot be restored; skipping.");
				return null;
			}

			var serialized = comp.Serialize(true);
			return new VisualScriptingBehaviourData {
				NodeId = nodeId,
				TypeName = comp.GetType().FullName,
				Json = serialized.json,
				ObjectRefs = MapObjectReferences(serialized.objectReferences, graphIds, graphQueue),
			};
		}

		private static VisualScriptingGraphData.GraphAsset PackGraph(ScriptableObject asset,
			Dictionary<int, string> graphIds, Queue<ScriptableObject> graphQueue)
		{
			var serialized = asset.Serialize(true);
			return new VisualScriptingGraphData.GraphAsset {
				Id = graphIds[asset.GetInstanceID()],
				TypeName = asset.GetType().FullName,
				Name = asset.name,
				Json = serialized.json,
				ObjectRefs = MapObjectReferences(serialized.objectReferences, graphIds, graphQueue),
			};
		}

		// Replaces each serialized UnityEngine.Object reference with a stable id. Graph-asset (macro)
		// references get an id and are enqueued for packing; anything else cannot be carried and is
		// written as null (index alignment is preserved so the remaining references still resolve).
		private static List<string> MapObjectReferences(UnityObject[] objectReferences,
			Dictionary<int, string> graphIds, Queue<ScriptableObject> graphQueue)
		{
			var result = new List<string>();
			if (objectReferences == null) {
				return result;
			}

			foreach (var obj in objectReferences) {
				if (obj == null) {
					result.Add(null);
					continue;
				}

				if (obj is IMacro && obj is ScriptableObject macro) {
					var instanceId = macro.GetInstanceID();
					if (!graphIds.TryGetValue(instanceId, out var id)) {
						id = "g" + graphIds.Count;
						graphIds[instanceId] = id;
						graphQueue.Enqueue(macro);
					}
					result.Add(id);
				} else {
					Debug.LogWarning($"[VS] A visual scripting graph references an unsupported object of " +
						$"type {obj.GetType().Name} ('{obj.name}'); it will not be restored.");
					result.Add(null);
				}
			}
			return result;
		}

		#endregion

		#region Unpack helpers

		private static void RestoreBehaviour(VisualScriptingBehaviourData behaviour, PackagedRefs refs,
			Dictionary<string, ScriptableObject> instanceById)
		{
			var type = VisualScriptingTypeResolver.Resolve(behaviour.TypeName);
			if (type == null) {
				Debug.LogError($"[VS] Cannot resolve component type '{behaviour.TypeName}'; skipping.");
				return;
			}

			var node = refs.GetNode(behaviour.NodeId);
			if (node == null) {
				Debug.LogError($"[VS] Cannot find node '{behaviour.NodeId}' for {type.Name}; skipping.");
				return;
			}

			var comp = node.GetComponent(type);
			if (comp == null) {
				comp = node.gameObject.AddComponent(type);
			}
			RestoreInto(comp, behaviour.Json, behaviour.ObjectRefs, instanceById, $"{type.Name} on '{node.name}'");
		}

		// Mirrors Unity's own load path (LudiqBehaviour/LudiqScriptableObject.OnAfterDeserialize ->
		// _data.DeserializeInto(ref this, true)).
		private static void RestoreInto(object instance, string json, List<string> objectRefs,
			Dictionary<string, ScriptableObject> instanceById, string label)
		{
			try {
				var data = new SerializationData(json, ResolveObjectReferences(objectRefs, instanceById));
				data.DeserializeInto(ref instance, true);
			} catch (Exception ex) {
				Debug.LogError($"[VS] Failed to restore {label}: {ex}");
			}
		}

		private static UnityObject[] ResolveObjectReferences(List<string> objectRefs,
			Dictionary<string, ScriptableObject> instanceById)
		{
			if (objectRefs == null || objectRefs.Count == 0) {
				return Array.Empty<UnityObject>();
			}

			var result = new UnityObject[objectRefs.Count];
			for (var i = 0; i < objectRefs.Count; i++) {
				if (objectRefs[i] != null && instanceById.TryGetValue(objectRefs[i], out var instance)) {
					result[i] = instance;
				}
			}
			return result;
		}

		#endregion
	}

	/// <summary>The packed visual scripting payload (see <see cref="VisualScriptingGraphPackager"/>).</summary>
	[Serializable]
	public class VisualScriptingGraphData
	{
		public int Version = 1;

		/// <summary>Object-scoped <c>Variables</c> components, keyed by node id.</summary>
		public List<VisualScriptingBehaviourData> Variables = new();

		/// <summary><c>ScriptMachine</c>/<c>StateMachine</c> components, keyed by node id.</summary>
		public List<VisualScriptingBehaviourData> Machines = new();

		/// <summary>The transitive closure of graph assets the machines reference.</summary>
		public List<GraphAsset> Graphs = new();

		[Serializable]
		public class GraphAsset
		{
			/// <summary>Stable id used to wire object references between graphs.</summary>
			public string Id;
			public string TypeName;
			public string Name;
			public string Json;
			public List<string> ObjectRefs = new();
		}
	}

	/// <summary>A serialized Visual Scripting <c>LudiqBehaviour</c> (machine or variables component).</summary>
	[Serializable]
	public class VisualScriptingBehaviourData
	{
		/// <summary>Stable node id of the GameObject the component lives on.</summary>
		public string NodeId;
		public string TypeName;
		public string Json;
		public List<string> ObjectRefs = new();
	}

	/// <summary>
	/// Resolves a type from its full name by scanning the loaded assemblies. Used to recreate the
	/// concrete Visual Scripting component/asset types (which live in Unity's packages) on import.
	/// </summary>
	internal static class VisualScriptingTypeResolver
	{
		private static readonly Dictionary<string, Type> Cache = new();

		public static Type Resolve(string fullName)
		{
			if (string.IsNullOrEmpty(fullName)) {
				return null;
			}
			if (Cache.TryGetValue(fullName, out var cached)) {
				return cached;
			}

			var type = Type.GetType(fullName);
			if (type == null) {
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
					type = assembly.GetType(fullName);
					if (type != null) {
						break;
					}
				}
			}

			Cache[fullName] = type;
			return type;
		}
	}
}
