using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	internal class GraphicsSettingsWindow
	{
		internal class BuiltinShaderSettings
		{
			internal enum BuiltinShaderMode
			{
				None,
				Builtin,
				Custom
			}

			private readonly SerializedProperty m_Mode;

			private readonly SerializedProperty m_Shader;

			private readonly GUIContent m_Label;

			internal BuiltinShaderSettings(string label, string name, SerializedObject serializedObject)
			{
				this.m_Mode = serializedObject.FindProperty(name + ".m_Mode");
				this.m_Shader = serializedObject.FindProperty(name + ".m_Shader");
				this.m_Label = EditorGUIUtility.TextContent(label);
			}

			internal void DoGUI()
			{
				EditorGUILayout.PropertyField(this.m_Mode, this.m_Label, new GUILayoutOption[0]);
				if (this.m_Mode.intValue == 2)
				{
					EditorGUILayout.PropertyField(this.m_Shader, new GUILayoutOption[0]);
				}
			}
		}

		internal class BuiltinShadersEditor : Editor
		{
			private GraphicsSettingsWindow.BuiltinShaderSettings m_Deferred;

			private GraphicsSettingsWindow.BuiltinShaderSettings m_DeferredReflections;

			private GraphicsSettingsWindow.BuiltinShaderSettings m_ScreenSpaceShadows;

			private GraphicsSettingsWindow.BuiltinShaderSettings m_LegacyDeferred;

			private GraphicsSettingsWindow.BuiltinShaderSettings m_MotionVectors;

			private string deferredString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Deferred|Shader settings for Deferred Shading");
				}
			}

			private string deferredReflString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Deferred Reflections|Shader settings for deferred reflections");
				}
			}

			private string legacyDeferredString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Legacy Deferred|Shader settings for Legacy (light prepass) Deferred Lighting");
				}
			}

			private string screenShadowsString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Screen Space Shadows|Shader settings for cascaded shadow maps");
				}
			}

			private string motionVectorsString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Motion Vectors|Shader for generation of Motion Vectors when the rendering camera has renderMotionVectors set to true");
				}
			}

			public void OnEnable()
			{
				this.m_Deferred = new GraphicsSettingsWindow.BuiltinShaderSettings(this.deferredString, "m_Deferred", base.serializedObject);
				this.m_DeferredReflections = new GraphicsSettingsWindow.BuiltinShaderSettings(this.deferredReflString, "m_DeferredReflections", base.serializedObject);
				this.m_ScreenSpaceShadows = new GraphicsSettingsWindow.BuiltinShaderSettings(this.screenShadowsString, "m_ScreenSpaceShadows", base.serializedObject);
				this.m_LegacyDeferred = new GraphicsSettingsWindow.BuiltinShaderSettings(this.legacyDeferredString, "m_LegacyDeferred", base.serializedObject);
				this.m_MotionVectors = new GraphicsSettingsWindow.BuiltinShaderSettings(this.motionVectorsString, "m_MotionVectors", base.serializedObject);
			}

			public override void OnInspectorGUI()
			{
				base.serializedObject.Update();
				this.m_Deferred.DoGUI();
				EditorGUI.BeginChangeCheck();
				this.m_DeferredReflections.DoGUI();
				if (EditorGUI.EndChangeCheck())
				{
					ShaderUtil.ReloadAllShaders();
				}
				this.m_ScreenSpaceShadows.DoGUI();
				this.m_LegacyDeferred.DoGUI();
				this.m_MotionVectors.DoGUI();
				base.serializedObject.ApplyModifiedProperties();
			}
		}

		internal class AlwaysIncludedShadersEditor : Editor
		{
			private SerializedProperty m_AlwaysIncludedShaders;

			public void OnEnable()
			{
				this.m_AlwaysIncludedShaders = base.serializedObject.FindProperty("m_AlwaysIncludedShaders");
				this.m_AlwaysIncludedShaders.isExpanded = true;
			}

			public override void OnInspectorGUI()
			{
				base.serializedObject.Update();
				EditorGUILayout.PropertyField(this.m_AlwaysIncludedShaders, true, new GUILayoutOption[0]);
				base.serializedObject.ApplyModifiedProperties();
			}
		}

		internal class ShaderStrippingEditor : Editor
		{
			internal class Styles
			{
				public static readonly GUIContent shaderSettings = EditorGUIUtility.TextContent("Platform shader settings");

				public static readonly GUIContent builtinSettings = EditorGUIUtility.TextContent("Built-in shader settings");

				public static readonly GUIContent shaderStrippingSettings = EditorGUIUtility.TextContent("Shader stripping");

				public static readonly GUIContent shaderPreloadSettings = EditorGUIUtility.TextContent("Shader preloading");

				public static readonly GUIContent lightmapModes = EditorGUIUtility.TextContent("Lightmap Modes");

				public static readonly GUIContent lightmapPlain = EditorGUIUtility.TextContent("Baked Non-Directional|Include support for baked non-directional lightmaps.");

				public static readonly GUIContent lightmapDirCombined = EditorGUIUtility.TextContent("Baked Directional|Include support for baked directional lightmaps.");

				public static readonly GUIContent lightmapKeepShadowMask = EditorGUIUtility.TextContent("Baked Shadow Mask|Include support for baked shadow occlusion.");

				public static readonly GUIContent lightmapKeepSubtractive = EditorGUIUtility.TextContent("Baked Subtractive|Include support for baked substractive lightmaps.");

				public static readonly GUIContent lightmapDynamicPlain = EditorGUIUtility.TextContent("Realtime Non-Directional|Include support for realtime non-directional lightmaps.");

				public static readonly GUIContent lightmapDynamicDirCombined = EditorGUIUtility.TextContent("Realtime Directional|Include support for realtime directional lightmaps.");

				public static readonly GUIContent lightmapFromScene = EditorGUIUtility.TextContent("From current scene|Calculate lightmap modes used by the current scene.");

				public static readonly GUIContent fogModes = EditorGUIUtility.TextContent("Fog Modes");

				public static readonly GUIContent fogLinear = EditorGUIUtility.TextContent("Linear|Include support for Linear fog.");

				public static readonly GUIContent fogExp = EditorGUIUtility.TextContent("Exponential|Include support for Exponential fog.");

				public static readonly GUIContent fogExp2 = EditorGUIUtility.TextContent("Exponential Squared|Include support for Exponential Squared fog.");

				public static readonly GUIContent fogFromScene = EditorGUIUtility.TextContent("From current scene|Calculate fog modes used by the current scene.");

				public static readonly GUIContent instancingVariants = EditorGUIUtility.TextContent("Instancing Variants");

				public static readonly GUIContent shaderPreloadSave = EditorGUIUtility.TextContent("Save to asset...|Save currently tracked shaders into a Shader Variant Manifest asset.");

				public static readonly GUIContent shaderPreloadClear = EditorGUIUtility.TextContent("Clear|Clear currently tracked shader variant information.");
			}

			private SerializedProperty m_LightmapStripping;

			private SerializedProperty m_LightmapKeepPlain;

			private SerializedProperty m_LightmapKeepDirCombined;

			private SerializedProperty m_LightmapKeepDynamicPlain;

			private SerializedProperty m_LightmapKeepDynamicDirCombined;

			private SerializedProperty m_LightmapKeepShadowMask;

			private SerializedProperty m_LightmapKeepSubtractive;

			private SerializedProperty m_FogStripping;

			private SerializedProperty m_FogKeepLinear;

			private SerializedProperty m_FogKeepExp;

			private SerializedProperty m_FogKeepExp2;

			private SerializedProperty m_InstancingStripping;

			public void OnEnable()
			{
				this.m_LightmapStripping = base.serializedObject.FindProperty("m_LightmapStripping");
				this.m_LightmapKeepPlain = base.serializedObject.FindProperty("m_LightmapKeepPlain");
				this.m_LightmapKeepDirCombined = base.serializedObject.FindProperty("m_LightmapKeepDirCombined");
				this.m_LightmapKeepDynamicPlain = base.serializedObject.FindProperty("m_LightmapKeepDynamicPlain");
				this.m_LightmapKeepDynamicDirCombined = base.serializedObject.FindProperty("m_LightmapKeepDynamicDirCombined");
				this.m_LightmapKeepShadowMask = base.serializedObject.FindProperty("m_LightmapKeepShadowMask");
				this.m_LightmapKeepSubtractive = base.serializedObject.FindProperty("m_LightmapKeepSubtractive");
				this.m_FogStripping = base.serializedObject.FindProperty("m_FogStripping");
				this.m_FogKeepLinear = base.serializedObject.FindProperty("m_FogKeepLinear");
				this.m_FogKeepExp = base.serializedObject.FindProperty("m_FogKeepExp");
				this.m_FogKeepExp2 = base.serializedObject.FindProperty("m_FogKeepExp2");
				this.m_InstancingStripping = base.serializedObject.FindProperty("m_InstancingStripping");
			}

			public override void OnInspectorGUI()
			{
				base.serializedObject.Update();
				bool flag = false;
				bool flag2 = false;
				EditorGUILayout.PropertyField(this.m_LightmapStripping, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.lightmapModes, new GUILayoutOption[0]);
				if (this.m_LightmapStripping.intValue != 0)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(this.m_LightmapKeepPlain, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.lightmapPlain, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LightmapKeepDirCombined, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.lightmapDirCombined, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LightmapKeepDynamicPlain, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.lightmapDynamicPlain, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LightmapKeepDynamicDirCombined, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.lightmapDynamicDirCombined, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LightmapKeepShadowMask, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.lightmapKeepShadowMask, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LightmapKeepSubtractive, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.lightmapKeepSubtractive, new GUILayoutOption[0]);
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					EditorGUILayout.PrefixLabel(GUIContent.Temp(" "), EditorStyles.miniButton);
					if (GUILayout.Button(GraphicsSettingsWindow.ShaderStrippingEditor.Styles.lightmapFromScene, EditorStyles.miniButton, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						flag = true;
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.PropertyField(this.m_FogStripping, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.fogModes, new GUILayoutOption[0]);
				if (this.m_FogStripping.intValue != 0)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(this.m_FogKeepLinear, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.fogLinear, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_FogKeepExp, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.fogExp, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_FogKeepExp2, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.fogExp2, new GUILayoutOption[0]);
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					EditorGUILayout.PrefixLabel(GUIContent.Temp(" "), EditorStyles.miniButton);
					if (GUILayout.Button(GraphicsSettingsWindow.ShaderStrippingEditor.Styles.fogFromScene, EditorStyles.miniButton, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						flag2 = true;
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.PropertyField(this.m_InstancingStripping, GraphicsSettingsWindow.ShaderStrippingEditor.Styles.instancingVariants, new GUILayoutOption[0]);
				base.serializedObject.ApplyModifiedProperties();
				if (flag)
				{
					ShaderUtil.CalculateLightmapStrippingFromCurrentScene();
				}
				if (flag2)
				{
					ShaderUtil.CalculateFogStrippingFromCurrentScene();
				}
			}
		}

		internal class ShaderPreloadEditor : Editor
		{
			internal class Styles
			{
				public static readonly GUIContent shaderPreloadSave = EditorGUIUtility.TextContent("Save to asset...|Save currently tracked shaders into a Shader Variant Manifest asset.");

				public static readonly GUIContent shaderPreloadClear = EditorGUIUtility.TextContent("Clear|Clear currently tracked shader variant information.");
			}

			private SerializedProperty m_PreloadedShaders;

			public void OnEnable()
			{
				this.m_PreloadedShaders = base.serializedObject.FindProperty("m_PreloadedShaders");
				this.m_PreloadedShaders.isExpanded = true;
			}

			public override void OnInspectorGUI()
			{
				base.serializedObject.Update();
				base.serializedObject.ApplyModifiedProperties();
				EditorGUILayout.PropertyField(this.m_PreloadedShaders, true, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				GUILayout.Label(string.Format("Currently tracked: {0} shaders {1} total variants", ShaderUtil.GetCurrentShaderVariantCollectionShaderCount(), ShaderUtil.GetCurrentShaderVariantCollectionVariantCount()), new GUILayoutOption[0]);
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(GraphicsSettingsWindow.ShaderPreloadEditor.Styles.shaderPreloadSave, EditorStyles.miniButton, new GUILayoutOption[0]))
				{
					string message = "Save shader variant collection";
					string text = EditorUtility.SaveFilePanelInProject("Save Shader Variant Collection", "NewShaderVariants", "shadervariants", message, ProjectWindowUtil.GetActiveFolderPath());
					if (!string.IsNullOrEmpty(text))
					{
						ShaderUtil.SaveCurrentShaderVariantCollection(text);
					}
					GUIUtility.ExitGUI();
				}
				if (GUILayout.Button(GraphicsSettingsWindow.ShaderPreloadEditor.Styles.shaderPreloadClear, EditorStyles.miniButton, new GUILayoutOption[0]))
				{
					ShaderUtil.ClearCurrentShaderVariantCollection();
				}
				EditorGUILayout.EndHorizontal();
				base.serializedObject.ApplyModifiedProperties();
			}
		}

		internal class TierSettingsEditor : Editor
		{
			internal class Styles
			{
				public static readonly GUIContent[] shaderQualityName = new GUIContent[]
				{
					new GUIContent("Low"),
					new GUIContent("Medium"),
					new GUIContent("High")
				};

				public static readonly int[] shaderQualityValue = new int[]
				{
					0,
					1,
					2
				};

				public static readonly GUIContent[] renderingPathName = new GUIContent[]
				{
					new GUIContent("Forward"),
					new GUIContent("Deferred"),
					new GUIContent("Legacy Vertex Lit"),
					new GUIContent("Legacy Deferred (light prepass)")
				};

				public static readonly int[] renderingPathValue = new int[]
				{
					1,
					3,
					0,
					2
				};

				public static readonly GUIContent[] hdrModeName = new GUIContent[]
				{
					new GUIContent("FP16"),
					new GUIContent("R11G11B10")
				};

				public static readonly int[] hdrModeValue = new int[]
				{
					1,
					2
				};

				public static readonly GUIContent[] realtimeGICPUUsageName = new GUIContent[]
				{
					new GUIContent("Low"),
					new GUIContent("Medium"),
					new GUIContent("High"),
					new GUIContent("Unlimited")
				};

				public static readonly int[] realtimeGICPUUsageValue = new int[]
				{
					25,
					50,
					75,
					100
				};

				public static readonly GUIContent[] tierName = new GUIContent[]
				{
					new GUIContent("Low (Tier1)"),
					new GUIContent("Medium (Tier 2)"),
					new GUIContent("High (Tier 3)")
				};

				public static readonly GUIContent empty = EditorGUIUtility.TextContent("");

				public static readonly GUIContent autoSettings = EditorGUIUtility.TextContent("Use Defaults");

				public static readonly GUIContent standardShaderSettings = EditorGUIUtility.TextContent("Standard Shader");

				public static readonly GUIContent renderingSettings = EditorGUIUtility.TextContent("Rendering");

				public static readonly GUIContent standardShaderQuality = EditorGUIUtility.TextContent("Standard Shader Quality");

				public static readonly GUIContent reflectionProbeBoxProjection = EditorGUIUtility.TextContent("Reflection Probes Box Projection");

				public static readonly GUIContent reflectionProbeBlending = EditorGUIUtility.TextContent("Reflection Probes Blending");

				public static readonly GUIContent detailNormalMap = EditorGUIUtility.TextContent("Detail Normal Map");

				public static readonly GUIContent cascadedShadowMaps = EditorGUIUtility.TextContent("Cascaded Shadows");

				public static readonly GUIContent semitransparentShadows = EditorGUIUtility.TextContent("Enable Semitransparent Shadows");

				public static readonly GUIContent renderingPath = EditorGUIUtility.TextContent("Rendering Path");

				public static readonly GUIContent useHDR = EditorGUIUtility.TextContent("Use HDR");

				public static readonly GUIContent hdrMode = EditorGUIUtility.TextContent("HDR Mode");

				public static readonly GUIContent realtimeGICPUUsage = EditorGUIUtility.TextContent("Realtime Global Illumination CPU Usage|How many CPU worker threads to create for Realtime Global Illumination lighting calculations in the Player. Increasing this makes the system react faster to changes in lighting at a cost of using more CPU time. The higher the CPU Usage value, the more worker threads are created for solving Realtime GI.");
			}

			public bool verticalLayout = false;

			internal void OnFieldLabelsGUI(bool vertical)
			{
				if (!vertical)
				{
					EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.standardShaderSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
				}
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.standardShaderQuality, new GUILayoutOption[0]);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.reflectionProbeBoxProjection, new GUILayoutOption[0]);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.reflectionProbeBlending, new GUILayoutOption[0]);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.detailNormalMap, new GUILayoutOption[0]);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.semitransparentShadows, new GUILayoutOption[0]);
				if (!vertical)
				{
					EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.empty, EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.renderingSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
				}
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.cascadedShadowMaps, new GUILayoutOption[0]);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.useHDR, new GUILayoutOption[0]);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.hdrMode, new GUILayoutOption[0]);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.renderingPath, new GUILayoutOption[0]);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.realtimeGICPUUsage, new GUILayoutOption[0]);
			}

			internal ShaderQuality ShaderQualityPopup(ShaderQuality sq)
			{
				return (ShaderQuality)EditorGUILayout.IntPopup((int)sq, GraphicsSettingsWindow.TierSettingsEditor.Styles.shaderQualityName, GraphicsSettingsWindow.TierSettingsEditor.Styles.shaderQualityValue, new GUILayoutOption[0]);
			}

			internal RenderingPath RenderingPathPopup(RenderingPath rp)
			{
				return (RenderingPath)EditorGUILayout.IntPopup((int)rp, GraphicsSettingsWindow.TierSettingsEditor.Styles.renderingPathName, GraphicsSettingsWindow.TierSettingsEditor.Styles.renderingPathValue, new GUILayoutOption[0]);
			}

			internal CameraHDRMode HDRModePopup(CameraHDRMode mode)
			{
				return (CameraHDRMode)EditorGUILayout.IntPopup((int)mode, GraphicsSettingsWindow.TierSettingsEditor.Styles.hdrModeName, GraphicsSettingsWindow.TierSettingsEditor.Styles.hdrModeValue, new GUILayoutOption[0]);
			}

			internal RealtimeGICPUUsage RealtimeGICPUUsagePopup(RealtimeGICPUUsage usage)
			{
				return (RealtimeGICPUUsage)EditorGUILayout.IntPopup((int)usage, GraphicsSettingsWindow.TierSettingsEditor.Styles.realtimeGICPUUsageName, GraphicsSettingsWindow.TierSettingsEditor.Styles.realtimeGICPUUsageValue, new GUILayoutOption[0]);
			}

			internal void OnTierGUI(BuildTargetGroup platform, GraphicsTier tier, bool vertical)
			{
				TierSettings tierSettings = EditorGraphicsSettings.GetTierSettings(platform, tier);
				EditorGUI.BeginChangeCheck();
				if (!vertical)
				{
					EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.empty, EditorStyles.boldLabel, new GUILayoutOption[0]);
				}
				tierSettings.standardShaderQuality = this.ShaderQualityPopup(tierSettings.standardShaderQuality);
				tierSettings.reflectionProbeBoxProjection = EditorGUILayout.Toggle(tierSettings.reflectionProbeBoxProjection, new GUILayoutOption[0]);
				tierSettings.reflectionProbeBlending = EditorGUILayout.Toggle(tierSettings.reflectionProbeBlending, new GUILayoutOption[0]);
				tierSettings.detailNormalMap = EditorGUILayout.Toggle(tierSettings.detailNormalMap, new GUILayoutOption[0]);
				tierSettings.semitransparentShadows = EditorGUILayout.Toggle(tierSettings.semitransparentShadows, new GUILayoutOption[0]);
				if (!vertical)
				{
					EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.empty, EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.empty, EditorStyles.boldLabel, new GUILayoutOption[0]);
				}
				tierSettings.cascadedShadowMaps = EditorGUILayout.Toggle(tierSettings.cascadedShadowMaps, new GUILayoutOption[0]);
				tierSettings.hdr = EditorGUILayout.Toggle(tierSettings.hdr, new GUILayoutOption[0]);
				tierSettings.hdrMode = this.HDRModePopup(tierSettings.hdrMode);
				tierSettings.renderingPath = this.RenderingPathPopup(tierSettings.renderingPath);
				tierSettings.realtimeGICPUUsage = this.RealtimeGICPUUsagePopup(tierSettings.realtimeGICPUUsage);
				if (EditorGUI.EndChangeCheck())
				{
					EditorGraphicsSettings.RegisterUndoForGraphicsSettings();
					EditorGraphicsSettings.SetTierSettings(platform, tier, tierSettings);
				}
			}

			internal void OnGuiHorizontal(BuildTargetGroup platform)
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				EditorGUIUtility.labelWidth = 140f;
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.empty, EditorStyles.boldLabel, new GUILayoutOption[0]);
				this.OnFieldLabelsGUI(false);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.empty, EditorStyles.boldLabel, new GUILayoutOption[0]);
				EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.autoSettings, EditorStyles.boldLabel, new GUILayoutOption[0]);
				EditorGUILayout.EndVertical();
				EditorGUIUtility.labelWidth = 50f;
				IEnumerator enumerator = Enum.GetValues(typeof(GraphicsTier)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						GraphicsTier graphicsTier = (GraphicsTier)enumerator.Current;
						bool flag = EditorGraphicsSettings.AreTierSettingsAutomatic(platform, graphicsTier);
						EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
						EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.tierName[(int)graphicsTier], EditorStyles.boldLabel, new GUILayoutOption[0]);
						using (new EditorGUI.DisabledScope(flag))
						{
							this.OnTierGUI(platform, graphicsTier, false);
						}
						EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.empty, EditorStyles.boldLabel, new GUILayoutOption[0]);
						EditorGUI.BeginChangeCheck();
						flag = EditorGUILayout.Toggle(flag, new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							EditorGraphicsSettings.RegisterUndoForGraphicsSettings();
							EditorGraphicsSettings.MakeTierSettingsAutomatic(platform, graphicsTier, flag);
							EditorGraphicsSettings.OnUpdateTierSettingsImpl(platform, true);
						}
						EditorGUILayout.EndVertical();
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				EditorGUIUtility.labelWidth = 0f;
				EditorGUILayout.EndHorizontal();
			}

			internal void OnGuiVertical(BuildTargetGroup platform)
			{
				IEnumerator enumerator = Enum.GetValues(typeof(GraphicsTier)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						GraphicsTier graphicsTier = (GraphicsTier)enumerator.Current;
						bool flag = EditorGraphicsSettings.AreTierSettingsAutomatic(platform, graphicsTier);
						EditorGUI.BeginChangeCheck();
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						EditorGUIUtility.labelWidth = 80f;
						EditorGUILayout.LabelField(GraphicsSettingsWindow.TierSettingsEditor.Styles.tierName[(int)graphicsTier], EditorStyles.boldLabel, new GUILayoutOption[0]);
						GUILayout.FlexibleSpace();
						EditorGUIUtility.labelWidth = 75f;
						flag = EditorGUILayout.Toggle(GraphicsSettingsWindow.TierSettingsEditor.Styles.autoSettings, flag, new GUILayoutOption[0]);
						GUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck())
						{
							EditorGraphicsSettings.RegisterUndoForGraphicsSettings();
							EditorGraphicsSettings.MakeTierSettingsAutomatic(platform, graphicsTier, flag);
							EditorGraphicsSettings.OnUpdateTierSettingsImpl(platform, true);
						}
						using (new EditorGUI.DisabledScope(flag))
						{
							EditorGUI.indentLevel++;
							EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
							EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
							EditorGUIUtility.labelWidth = 140f;
							this.OnFieldLabelsGUI(true);
							EditorGUILayout.EndVertical();
							EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
							EditorGUIUtility.labelWidth = 50f;
							this.OnTierGUI(platform, graphicsTier, true);
							EditorGUILayout.EndVertical();
							GUILayout.EndHorizontal();
							EditorGUI.indentLevel--;
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				EditorGUIUtility.labelWidth = 0f;
			}

			public override void OnInspectorGUI()
			{
				BuildPlayerWindow.BuildPlatform[] array = BuildPlayerWindow.GetValidPlatforms().ToArray();
				BuildTargetGroup targetGroup = array[EditorGUILayout.BeginPlatformGrouping(array, null, GUIStyle.none)].targetGroup;
				if (this.verticalLayout)
				{
					this.OnGuiVertical(targetGroup);
				}
				else
				{
					this.OnGuiHorizontal(targetGroup);
				}
				EditorGUILayout.EndPlatformGrouping();
			}
		}
	}
}
