/**
	TextFx Variable Progression Classes.
	Used in animation Actions to define either a constant value, or an ordered or random sequence of values within a given range.
**/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
#if UNITY_EDITOR
using UnityEditor;
using System;
#endif

namespace TextFx.LegacyContent
{
	public enum ValueProgression
	{
		Constant,
		Random,
		Eased,
		EasedCustom
	}

	[System.Serializable]
	public abstract class ActionVariableProgression
	{
	#if UNITY_EDITOR
		protected const float LINE_HEIGHT = 20;
		protected const float VECTOR_3_WIDTH = 300;
		protected const float PROGRESSION_HEADER_LABEL_WIDTH = 150;
		protected const float ACTION_INDENT_LEVEL_1 = 10;
		protected const float ENUM_SELECTOR_WIDTH = 300;
		protected const float ENUM_SELECTOR_WIDTH_MEDIUM = 120;
		protected const float ENUM_SELECTOR_WIDTH_SMALL = 70;
		
		protected static int[] PROGRESSION_ENUM_VALUES = new int[] {0,1,2,3};
	#endif
		
		
		[SerializeField]
		protected ValueProgression m_progression = ValueProgression.Constant;		// Legacy field
		[SerializeField]
		protected int m_progression_idx = -1;
		public virtual string[] ProgressionExtraOptions { get { return null; } }
		public virtual int[] ProgressionExtraOptionIndexes { get { return null; } }
		
		[SerializeField]
		protected EasingEquation m_ease_type = EasingEquation.Linear;
		[SerializeField]
		protected bool m_is_offset_from_last = false;
		[SerializeField]
		protected bool m_to_to_bool = false;
		[SerializeField]
		protected bool m_unique_randoms = false;
		[SerializeField]
		protected AnimatePerOptions m_animate_per;
		[SerializeField]
		protected bool m_override_animate_per_option = false;
		[SerializeField]
		protected AnimationCurve m_custom_ease_curve = new AnimationCurve();
		
		public EasingEquation EaseType { get { return m_ease_type; } }
		public bool IsOffsetFromLast { get { return m_is_offset_from_last; } set { m_is_offset_from_last = value; } }
		public bool UsingThirdValue { get { return m_to_to_bool; } }
		public AnimatePerOptions AnimatePer { get { return m_animate_per; } set { m_animate_per = value; } }
		public bool OverrideAnimatePerOption { get { return m_override_animate_per_option; } set { m_override_animate_per_option = value; } }
		public AnimationCurve CustomEaseCurve { get { return m_custom_ease_curve; } }
		public bool UniqueRandom { get { return Progression == (int) ValueProgression.Random && m_unique_randoms; } }
		public int Progression {
			get {
				if(m_progression_idx == -1)
					m_progression_idx = (int) m_progression;
				return m_progression_idx;
			}
		}
		
		public int GetProgressionIndex(AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per_default)
		{
			return progression_variables.GetValue(m_override_animate_per_option ? m_animate_per : animate_per_default);
		}

		protected void ExportBaseData(ref JSONObject json_data)
		{
			json_data["m_progression"] = Progression;
			json_data["m_ease_type"] = (int) m_ease_type;
			json_data["m_is_offset_from_last"] = m_is_offset_from_last;
			json_data["m_to_to_bool"] = m_to_to_bool;
			json_data["m_unique_randoms"] = m_unique_randoms;
			json_data["m_animate_per"] = (int) m_animate_per;
			json_data["m_override_animate_per_option"] = m_override_animate_per_option;
			
			if(Progression == (int) ValueProgression.EasedCustom)
			{
				json_data["m_custom_ease_curve"] = m_custom_ease_curve.ExportData();
			}
		}

		protected void ImportBaseData(JSONObject json_data)
		{
			m_progression_idx = (int) json_data["m_progression"].Number;
			m_ease_type = (EasingEquation) (int) json_data["m_ease_type"].Number;
			m_is_offset_from_last = json_data["m_is_offset_from_last"].Boolean;
			m_to_to_bool = json_data["m_to_to_bool"].Boolean;
			m_unique_randoms = json_data["m_unique_randoms"].Boolean;
			m_animate_per = (AnimatePerOptions) (int) json_data["m_animate_per"].Number;
			m_override_animate_per_option = json_data["m_override_animate_per_option"].Boolean;
			if(json_data.ContainsKey("m_custom_ease_curve"))
				m_custom_ease_curve = json_data["m_custom_ease_curve"].Array.JSONtoAnimationCurve();
		}

		public abstract JSONValue ExportData();

		public abstract void ImportData(JSONObject json_data);
		
		public void ImportBaseLagacyData(KeyValuePair<string, string> value_pair)
		{
			switch(value_pair.Key)
			{
				case "m_progression": m_progression_idx = int.Parse(value_pair.Value); break;
				case "m_ease_type": m_ease_type = (EasingEquation) int.Parse(value_pair.Value); break;
				case "m_is_offset_from_last": m_is_offset_from_last = bool.Parse(value_pair.Value); break;
				case "m_to_to_bool": m_to_to_bool = bool.Parse(value_pair.Value); break;
				case "m_unique_randoms": m_unique_randoms = bool.Parse(value_pair.Value); break;
				case "m_animate_per": m_animate_per = (AnimatePerOptions) int.Parse(value_pair.Value); break;
				case "m_override_animate_per_option": m_override_animate_per_option = bool.Parse(value_pair.Value); break;
				case "m_custom_ease_curve": m_custom_ease_curve = value_pair.Value.ToAnimationCurve(); break;
			}
		}
		
	#if UNITY_EDITOR	
		public float DrawProgressionEditorHeader(GUIContent label, Rect position, bool offset_legal, bool unique_randoms_legal, bool bold_label = true, string[] extra_options = null, int[] extra_option_indexes = null)
		{
			float x_offset = position.x;
			float y_offset = position.y;
			if(bold_label)
			{
				EditorGUI.LabelField(new Rect(x_offset, y_offset, position.width, LINE_HEIGHT), label, EditorStyles.boldLabel);
			}
			else
			{
				EditorGUI.LabelField(new Rect(x_offset, y_offset, position.width, LINE_HEIGHT), label);
			}
			x_offset += PROGRESSION_HEADER_LABEL_WIDTH;
			
			string[] options = Enum.GetNames( typeof(ValueProgression) );
			int[] option_indexes = PROGRESSION_ENUM_VALUES;
			
			if(extra_options != null && extra_option_indexes != null && extra_options.Length > 0 && extra_options.Length == extra_option_indexes.Length)
			{
				int original_length = options.Length;
				Array.Resize<string>(ref options, options.Length + extra_options.Length);
				Array.Copy(extra_options, 0, options, original_length, extra_options.Length);
				
				original_length = option_indexes.Length;
				Array.Resize<int>(ref option_indexes, option_indexes.Length + extra_option_indexes.Length);
				Array.Copy(extra_option_indexes, 0, option_indexes, original_length, extra_option_indexes.Length);
			}
			
			m_progression_idx = EditorGUI.IntPopup(new Rect(x_offset, y_offset, ENUM_SELECTOR_WIDTH_SMALL + 18, LINE_HEIGHT), Progression, options, option_indexes);
			x_offset += ENUM_SELECTOR_WIDTH_SMALL + 25;
			
			if(m_progression_idx == (int) ValueProgression.Eased)
			{
				EditorGUI.LabelField(new Rect(x_offset, y_offset, position.width, LINE_HEIGHT), new GUIContent("Function :", "Easing function used to lerp values between 'from' and 'to'."));
				x_offset += 65;
				m_ease_type = (EasingEquation) EditorGUI.EnumPopup(new Rect(x_offset, y_offset, ENUM_SELECTOR_WIDTH_MEDIUM, LINE_HEIGHT), m_ease_type);
				x_offset += ENUM_SELECTOR_WIDTH_MEDIUM + 10;
				
				EditorGUI.LabelField(new Rect(x_offset, y_offset, position.width, LINE_HEIGHT), new GUIContent("3rd?", "Option to add a third state to lerp values between."));
				x_offset += 35;
				m_to_to_bool = EditorGUI.Toggle(new Rect(x_offset, y_offset, ENUM_SELECTOR_WIDTH_MEDIUM, LINE_HEIGHT), m_to_to_bool);
			}
			else if(m_progression_idx == (int) ValueProgression.Random && unique_randoms_legal)
			{
				m_unique_randoms = EditorGUI.Toggle(new Rect(x_offset, y_offset, 200, LINE_HEIGHT), new GUIContent("Unique Randoms?", "Denotes whether a new random value will be picked each time this action is repeated (like when in a loop)."), m_unique_randoms);
			}
			y_offset += LINE_HEIGHT;
			
			if(offset_legal)
			{
				m_is_offset_from_last = EditorGUI.Toggle(new Rect(position.x + ACTION_INDENT_LEVEL_1, y_offset, 200, LINE_HEIGHT), new GUIContent("Offset From Last?", "Denotes whether this value will offset from whatever value it had in the last state. End states offset the start state. Start states offset the previous actions end state."), m_is_offset_from_last);
				y_offset += LINE_HEIGHT;
			}
			
			if((m_progression_idx == (int) ValueProgression.Eased || m_progression_idx == (int) ValueProgression.Random))
			{
				m_override_animate_per_option = EditorGUI.Toggle(new Rect(position.x + ACTION_INDENT_LEVEL_1, y_offset, 200, LINE_HEIGHT), new GUIContent("Override AnimatePer?", "Denotes whether this state value progression will use the global 'Animate Per' setting, or define its own."), m_override_animate_per_option);
				if(m_override_animate_per_option)
				{
					m_animate_per = (AnimatePerOptions) EditorGUI.EnumPopup(new Rect(position.x + ACTION_INDENT_LEVEL_1 + 200, y_offset, ENUM_SELECTOR_WIDTH_SMALL, LINE_HEIGHT), m_animate_per);
				}
				
				y_offset += LINE_HEIGHT;
			}
			else
			{
				m_override_animate_per_option = false;
			}
			
			return position.y + (y_offset - position.y);
		}
	#endif
	}

	[System.Serializable]
	public class ActionFloatProgression : ActionVariableProgression
	{
		[SerializeField]
		float[] m_values;
		[SerializeField]
		float m_from = 0;
		[SerializeField]
		float m_to = 0;
		[SerializeField]
		float m_to_to = 0;
		
		public float ValueFrom { get { return m_from; } }
		public float ValueTo { get { return m_to; } }
		public float ValueThen { get { return m_to_to; } }
		public float[] Values { get { return m_values; } set { m_values = value; } }
		
		public void SetConstant( float constant_value )
		{
			m_progression_idx = (int) ValueProgression.Constant;
			m_from = constant_value;
		}
		
		public void SetRandom( float random_min, float random_max, bool unique_randoms = false)
		{
			m_progression_idx = (int) ValueProgression.Random;
			m_from = random_min;
			m_to = random_max;
			m_unique_randoms = unique_randoms;
		}
		
		public void SetEased( EasingEquation easing_function, float eased_from, float eased_to)
		{
			m_progression_idx = (int) ValueProgression.Eased;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to_bool = false;
			m_ease_type = easing_function;
		}
		
		public void SetEased( EasingEquation easing_function, float eased_from, float eased_to, float eased_then)
		{
			m_progression_idx = (int) ValueProgression.Eased;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to = eased_then;
			m_to_to_bool = true;
			m_ease_type = easing_function;
		}
		
		public void SetEasedCustom ( AnimationCurve easing_curve, float eased_from, float eased_to)
		{
			m_progression_idx = (int) ValueProgression.EasedCustom;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to_bool = false;
			m_custom_ease_curve = easing_curve;
		}
		
		
		public float GetValue(AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per_default)
		{
			return GetValue(GetProgressionIndex(progression_variables,animate_per_default));
		}
		
		public float GetValue(int progression_idx)
		{
			int num_vals = m_values.Length;
			if(num_vals > 1 && progression_idx < num_vals)
			{
				return m_values[progression_idx];
			}
			else if(num_vals==1)
			{
				return m_values[0];
			}
			else
			{
				return 0;
			}
		}
		
	#if UNITY_EDITOR
		public int NumEditorLines
		{
			get
			{
				if(Progression == (int) ValueProgression.Constant)
				{
					return 2;
				}
				else if(Progression == (int) ValueProgression.Random || Progression == (int) ValueProgression.EasedCustom || (Progression == (int) ValueProgression.Eased && !m_to_to_bool))
				{
					return 4;
				}
				else
				{
					return 5;
				}
			}
		}
	#endif
		
		public ActionFloatProgression(float start_val)
		{
			m_from = start_val;
			m_to = start_val;
			m_to_to = start_val;
		}
		
		public void CalculateUniqueRandom(AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per)
		{
			m_values[GetProgressionIndex(progression_variables, animate_per)] = m_from + (m_to - m_from) * UnityEngine.Random.value;
		}
		
		public void CalculateProgressions(int num_progressions)
		{
			
			// Initialise array of values.
			m_values = new float[Progression == (int) ValueProgression.Eased || Progression == (int) ValueProgression.EasedCustom || Progression == (int) ValueProgression.Random
									? num_progressions
									: 1];
			
			if(Progression == (int) ValueProgression.Random) //  && (progression >= 0 || m_unique_randoms))
			{
				for(int idx=0; idx < num_progressions; idx++)
				{
					m_values[idx] = m_from + (m_to - m_from) * UnityEngine.Random.value;
				}
			}
			else if(Progression == (int) ValueProgression.Eased)
			{
				float progression;
				for(int idx=0; idx < num_progressions; idx++)
				{
					progression = num_progressions == 1 ? 0 : (float)idx / ((float)num_progressions - 1f);
					
					if(m_to_to_bool)
					{
						if(progression <= 0.5f)
						{
							m_values[idx] = m_from + (m_to - m_from) * EasingManager.GetEaseProgress(m_ease_type, progression/0.5f);
						}
						else
						{
							progression -= 0.5f;
							m_values[idx] = m_to + (m_to_to - m_to) * EasingManager.GetEaseProgress(EasingManager.GetEaseTypeOpposite(m_ease_type), progression/0.5f);
						}
					}
					else
					{
						m_values[idx] = m_from + (m_to - m_from) * EasingManager.GetEaseProgress(m_ease_type, progression);
					}
				}
			}
			else if(Progression == (int) ValueProgression.EasedCustom)
			{
				float progression;
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					progression = num_progressions == 1 ? 0 : (float)idx / ((float)num_progressions - 1f);
					
					m_values[idx] += m_from + (m_to - m_from) * m_custom_ease_curve.Evaluate(progression);
				}
			}
			else if(Progression == (int) ValueProgression.Constant)
			{
				m_values[0] = m_from;
			}
		}
		
		public ActionFloatProgression Clone()
		{
			ActionFloatProgression float_progression = new ActionFloatProgression(0);
			
			float_progression.m_progression_idx = Progression;
			float_progression.m_ease_type = m_ease_type;
			float_progression.m_from = m_from;
			float_progression.m_to = m_to;
			float_progression.m_to_to = m_to_to;
			float_progression.m_to_to_bool = m_to_to_bool;
			float_progression.m_unique_randoms = m_unique_randoms;
			float_progression.m_override_animate_per_option = m_override_animate_per_option;
			float_progression.m_animate_per = m_animate_per;
			
			return float_progression;
		}

		public override JSONValue ExportData()
		{
			JSONObject json_data = new JSONObject();
			
			ExportBaseData(ref json_data);
			
			json_data["m_from"] = m_from;
			json_data["m_to"] = m_to;
			json_data["m_to_to"] = m_to_to;
			
			return new JSONValue(json_data);
		}

		public override void ImportData(JSONObject json_data)
		{
			m_from = (float) json_data["m_from"].Number;
			m_to = (float) json_data["m_to"].Number;
			m_to_to = (float) json_data["m_to_to"].Number;
			
			ImportBaseData(json_data);
		}
		
		public void ImportLegacyData(string data_string)
		{
			KeyValuePair<string, string> value_pair;
			List<object> obj_list = data_string.StringToList(';',':');
			
			foreach(object obj in obj_list)
			{
				value_pair = (KeyValuePair<string, string>) obj;
				
				switch(value_pair.Key)
				{
					case "m_from": m_from = float.Parse(value_pair.Value); break;
					case "m_to": m_to = float.Parse(value_pair.Value); break;
					case "m_to_to": m_to_to = float.Parse(value_pair.Value); break;

					default :
						ImportBaseLagacyData(value_pair); break;
				}
			}
		}
		
	#if UNITY_EDITOR	
		public float DrawEditorGUI(GUIContent label, Rect position, bool offset_legal, bool unique_random_legal = false, bool bold_label = true)
		{
			float x_offset = position.x + ACTION_INDENT_LEVEL_1;
			float y_offset = DrawProgressionEditorHeader(label, position, offset_legal, unique_random_legal, bold_label, extra_options : ProgressionExtraOptions, extra_option_indexes : ProgressionExtraOptionIndexes);
			
			m_from = EditorGUI.FloatField(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), Progression == (int) ValueProgression.Constant ? "Value" : "Value From", m_from);
			y_offset += LINE_HEIGHT;
			
			if(Progression != (int) ValueProgression.Constant)
			{
				m_to = EditorGUI.FloatField(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Value To", m_to);
				y_offset += LINE_HEIGHT;
				
				if(Progression == (int) ValueProgression.Eased && m_to_to_bool)
				{
					m_to_to = EditorGUI.FloatField(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Value Then", m_to_to);
					y_offset += LINE_HEIGHT;
				}
				
				
				if(Progression == (int) ValueProgression.EasedCustom)
				{
					m_custom_ease_curve = EditorGUI.CurveField(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Ease Curve", m_custom_ease_curve );
					y_offset += LINE_HEIGHT * 1.2f;
				}
			}
			
			return (y_offset) - position.y;
		}
	#endif
	}

	[System.Serializable]
	public class ActionPositionVector3Progression : ActionVector3Progression
	{
		public const string CURVE_OPTION_STRING = "Curve";
		public const int CURVE_OPTION_INDEX = 4;
		public static string[] EXTRA_OPTION_STRINGS = new string[]{CURVE_OPTION_STRING};
		public static int[] EXTRA_OPTION_INDEXS = new int[]{CURVE_OPTION_INDEX};
		public override string[] ProgressionExtraOptions { get { return EXTRA_OPTION_STRINGS; } }
		public override int[] ProgressionExtraOptionIndexes { get { return EXTRA_OPTION_INDEXS; } }
		
		[SerializeField]
		TextFxBezierCurve m_bezier_curve = new TextFxBezierCurve();
		[SerializeField]
		bool m_force_position_override = false;
		
		public TextFxBezierCurve BezierCurve { get { return m_bezier_curve; } }
		public bool ForcePositionOverride { get { return m_force_position_override; } }
		
		public void SetConstant( Vector3 constant_value, bool force_this_position = false )
		{
			m_progression_idx = (int) ValueProgression.Constant;
			m_from = constant_value;
			m_force_position_override = force_this_position;
		}
		
		public void SetBezierCurve ( TextFxBezierCurve bezier_curve )
		{
			m_progression_idx = CURVE_OPTION_INDEX;
			m_bezier_curve = bezier_curve;
		}
		
		public void SetBezierCurve ( params Vector3[] curve_points )
		{
			m_progression_idx = CURVE_OPTION_INDEX;
			
			TextFxBezierCurve bezier_curve = new TextFxBezierCurve();
			bezier_curve.m_anchor_points = new List<BezierCurvePoint>();
			
			BezierCurvePoint curve_point = null;
			int idx=0;
			foreach(Vector3 point in curve_points)
			{
				if(idx % 2 == 0)
				{
					curve_point = new BezierCurvePoint();
					curve_point.m_anchor_point = point;
				}
				else
				{
					curve_point.m_handle_point = point;
					bezier_curve.m_anchor_points.Add(curve_point);
				}
				
				idx++;
			}
			
			if(idx % 2 == 1)
			{
				curve_point.m_handle_point = curve_point.m_anchor_point;
				bezier_curve.m_anchor_points.Add(curve_point);
			}
			
			m_bezier_curve = bezier_curve;
		}
		
	#if UNITY_EDITOR	
		public override int NumEditorLines
		{
			get
			{
				if(Progression == (int) ValueProgression.Constant)
				{
					return 4;
				}
				else if(Progression == (int) ValueProgression.Random || Progression == (int) ValueProgression.EasedCustom)
				{
					return 7;
				}
				else if(Progression == CURVE_OPTION_INDEX)
				{
					return 2 + (m_bezier_curve.EditorVisible ? 1 + ((m_bezier_curve.m_anchor_points != null ? m_bezier_curve.m_anchor_points.Count : 0) * 2) : 0);
				}
				else
				{
					return m_to_to_bool ? 8 : 6;
				}
			}
		}
	#endif
		
		public ActionPositionVector3Progression(Vector3 start_vec)
		{
			m_from = start_vec;
			m_to = start_vec;
			m_to_to = start_vec;
		}
		
		public ActionPositionVector3Progression CloneThis()
		{
			ActionPositionVector3Progression progression = new ActionPositionVector3Progression(Vector3.zero);
			
			progression.m_progression_idx = Progression;
			progression.m_ease_type = m_ease_type;
			progression.m_from = m_from;
			progression.m_to = m_to;
			progression.m_to_to = m_to_to;
			progression.m_to_to_bool = m_to_to_bool;
			progression.m_is_offset_from_last = m_is_offset_from_last;
			progression.m_unique_randoms = m_unique_randoms;
			progression.m_force_position_override = m_force_position_override;
			progression.m_override_animate_per_option = m_override_animate_per_option;
			progression.m_animate_per = m_animate_per;
			progression.m_ease_curve_per_axis = m_ease_curve_per_axis;
			progression.m_custom_ease_curve = new AnimationCurve(m_custom_ease_curve.keys);
			progression.m_custom_ease_curve_y = new AnimationCurve(m_custom_ease_curve_y.keys);
			progression.m_custom_ease_curve_z = new AnimationCurve(m_custom_ease_curve_z.keys);
			progression.m_bezier_curve = new TextFxBezierCurve(progression.m_bezier_curve);
			
			return progression;
		}
		
		public void CalculatePositionProgressions(ref float[] letter_progressions, int num_progressions, Vector3[] offset_vecs, bool force_offset_from_last = false)
		{
			if(Progression == CURVE_OPTION_INDEX)
			{
				bool constant_offset = offset_vecs != null && offset_vecs.Length == 1;
				m_values = new Vector3[num_progressions];
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					m_values[idx] = m_is_offset_from_last ? offset_vecs[constant_offset ? 0 : idx] : Vector3.zero;
				}
				
				for(int idx=0; idx < letter_progressions.Length; idx++)
				{
					m_values[idx] += m_bezier_curve.GetCurvePoint(letter_progressions[idx]);
				}
			}
			else
			{
				CalculateProgressions(num_progressions, offset_vecs);
			}
		}

		public override JSONValue ExportData()
		{
			JSONObject json_data = base.ExportData().Obj;
			
			json_data["m_force_position_override"] = m_force_position_override;
			if(Progression == CURVE_OPTION_INDEX)
			{
				json_data["m_bezier_curve"] = m_bezier_curve.ExportData();
			}
			
			return new JSONValue(json_data);
		}
		
		public override void ImportData(JSONObject json_data)
		{
			base.ImportData(json_data);
			
			m_force_position_override = json_data["m_force_position_override"].Boolean;
			
			if(json_data.ContainsKey("m_bezier_curve"))
			{
				m_bezier_curve.ImportData(json_data["m_bezier_curve"].Obj);
			}
		}
		
	#if UNITY_EDITOR
		public float DrawPositionEditorGUI(GUIContent label, Rect position, bool offset_legal, bool unique_random_legal = false, bool bold_label = true)
		{
			float x_offset = position.x + ACTION_INDENT_LEVEL_1;
			float y_offset = DrawProgressionEditorHeader(label, position, offset_legal, unique_random_legal, bold_label, extra_options : ProgressionExtraOptions, extra_option_indexes : ProgressionExtraOptionIndexes);
			
			if(Progression == ActionPositionVector3Progression.CURVE_OPTION_INDEX)
			{
				// Handle displaying Bezier Curve position setup options
				TextFxBezierCurve bezier_curve = m_bezier_curve;
				
				bezier_curve.EditorVisible = EditorGUI.Foldout(new Rect(x_offset, y_offset, 300, LINE_HEIGHT), bezier_curve.EditorVisible, new GUIContent("Anchor Points" + (bezier_curve.EditorVisible ? "  [Scene View Debug]" : "")), true);
				y_offset += LINE_HEIGHT;
				
				if(bezier_curve.EditorVisible)
				{
					x_offset += 15;
					
					if(GUI.Button(new Rect(x_offset, y_offset, 120, LINE_HEIGHT), "Add Point"))
					{
						bezier_curve.AddNewAnchor();
					}
					y_offset += LINE_HEIGHT;
					
					if(bezier_curve.m_anchor_points != null)
					{
						for(int idx=0; idx < bezier_curve.m_anchor_points.Count; idx++)
						{
							m_bezier_curve.m_anchor_points[idx].m_anchor_point = EditorGUI.Vector3Field(new Rect(x_offset, y_offset, 200, LINE_HEIGHT), "Anchor #" + idx, m_bezier_curve.m_anchor_points[idx].m_anchor_point);
							m_bezier_curve.m_anchor_points[idx].m_handle_point = EditorGUI.Vector3Field(new Rect(x_offset + 210, y_offset, 200, LINE_HEIGHT), "Handle #" + idx, m_bezier_curve.m_anchor_points[idx].m_handle_point);
							
							if(GUI.Button(new Rect(x_offset - 23, y_offset + 12, 20, LINE_HEIGHT), "X"))
							{
								m_bezier_curve.m_anchor_points.RemoveAt(idx);
								idx--;
								break;
							}
							
							y_offset += LINE_HEIGHT*2;
						}
					}
				}
				
				return (y_offset) - position.y;
			}
			
			
			if(Progression != (int) ValueProgression.Eased)
			{
				Rect toggle_pos = new Rect();
				if(offset_legal)
				{
					toggle_pos = new Rect(x_offset + 190, y_offset - LINE_HEIGHT, 200, LINE_HEIGHT);
				}
				else
				{
					toggle_pos = new Rect(x_offset, y_offset, 200, LINE_HEIGHT);
					
					y_offset += LINE_HEIGHT;
				}
				m_force_position_override = EditorGUI.Toggle(toggle_pos, "Force This Position?", m_force_position_override);
			}
			
			m_from = EditorGUI.Vector3Field(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), Progression == (int) ValueProgression.Constant ? "Vector" : "Vector From", m_from);
			y_offset += LINE_HEIGHT*2;
			
			if(Progression != (int) ValueProgression.Constant)
			{
				m_to = EditorGUI.Vector3Field(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Vector To", m_to);
				y_offset += LINE_HEIGHT*2;
				
				if(Progression == (int) ValueProgression.Eased && m_to_to_bool)
				{
					m_to_to = EditorGUI.Vector3Field(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Vector Then", m_to_to);
					y_offset += LINE_HEIGHT*2;
				}
				
				y_offset = DrawVector3CustomEaseCurveSettings(x_offset, y_offset);
			}
			
			return (y_offset) - position.y;
		}
	#endif
	}

	[System.Serializable]
	public class ActionVector3Progression : ActionVariableProgression
	{
		[SerializeField]
		protected Vector3[] m_values;
		[SerializeField]
		protected Vector3 m_from = Vector3.zero;
		[SerializeField]
		protected Vector3 m_to = Vector3.zero;
		[SerializeField]
		protected Vector3 m_to_to = Vector3.zero;
		
		[SerializeField]
		protected bool m_ease_curve_per_axis = false;
		[SerializeField]
		protected AnimationCurve m_custom_ease_curve_y = new AnimationCurve();
		[SerializeField]
		protected AnimationCurve m_custom_ease_curve_z = new AnimationCurve();
		
		public Vector3 ValueFrom { get { return m_from; } }
		public Vector3 ValueTo { get { return m_to; } }
		public Vector3 ValueThen { get { return m_to_to; } }
		public Vector3[] Values { get { return m_values; } set { m_values = value; } }
		
		public bool EaseCurvePerAxis { get { return m_ease_curve_per_axis; } }
		public AnimationCurve CustomEaseCurveY { get { return m_custom_ease_curve_y; } }
		public AnimationCurve CustomEaseCurveZ { get { return m_custom_ease_curve_z; } }
		
		
		public void SetConstant( Vector3 constant_value )
		{
			m_progression_idx = (int) ValueProgression.Constant;
			m_from = constant_value;
		}
		
		public void SetRandom( Vector3 random_min, Vector3 random_max, bool unique_randoms = false)
		{
			m_progression_idx = (int) ValueProgression.Random;
			m_from = random_min;
			m_to = random_max;
			m_unique_randoms = unique_randoms;
		}
		
		public void SetEased( EasingEquation easing_function, Vector3 eased_from, Vector3 eased_to)
		{
			m_progression_idx = (int) ValueProgression.Eased;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to_bool = false;
			m_ease_type = easing_function;
		}
		
		public void SetEased( EasingEquation easing_function, Vector3 eased_from, Vector3 eased_to, Vector3 eased_then)
		{
			m_progression_idx = (int) ValueProgression.Eased;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to = eased_then;
			m_to_to_bool = true;
			m_ease_type = easing_function;
		}
		
		public void SetEasedCustom ( AnimationCurve easing_curve, Vector3 eased_from, Vector3 eased_to)
		{
			m_progression_idx = (int) ValueProgression.EasedCustom;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to_bool = false;
			
			m_ease_curve_per_axis = false;
			m_custom_ease_curve = easing_curve;
		}
		
		public void SetEasedCustom ( AnimationCurve easing_curve_x, AnimationCurve easing_curve_y, AnimationCurve easing_curve_z, Vector3 eased_from, Vector3 eased_to)
		{
			m_progression_idx = (int) ValueProgression.EasedCustom;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to_bool = false;
			
			m_ease_curve_per_axis = true;
			m_custom_ease_curve = easing_curve_x;
			m_custom_ease_curve_y = easing_curve_y;
			m_custom_ease_curve_z = easing_curve_z;
		}
		
		
		
		public Vector3 GetValue(AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per_default)
		{
			return GetValue(GetProgressionIndex(progression_variables,animate_per_default));
		}
		
		Vector3 GetValue(int progression_idx)
		{
			int num_vals = m_values.Length;
			if(num_vals > 1 && progression_idx < num_vals)
			{
				return m_values[progression_idx];
			}
			else if(num_vals==1)
			{
				return m_values[0];
			}
			else
			{
				return Vector3.zero;
			}
		}
		
	#if UNITY_EDITOR
		public virtual int NumEditorLines
		{
			get
			{
				if(Progression == (int) ValueProgression.Constant)
				{
					return 3;
				}
				else if(Progression == (int) ValueProgression.Random || Progression == (int) ValueProgression.EasedCustom || (Progression == (int) ValueProgression.Eased && !m_to_to_bool))
				{
					return 6;
				}
				else
				{
					return 8;
				}
			}
		}
	#endif
		
		public ActionVector3Progression()
		{
			
		}
		
		public ActionVector3Progression(Vector3 start_vec)
		{
			m_from = start_vec;
			m_to = start_vec;
			m_to_to = start_vec;
		}
		
		public void CalculateUniqueRandom(AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per, Vector3[] offset_vec)
		{
			int progression_idx = GetProgressionIndex(progression_variables, animate_per);
			bool constant_offset = offset_vec != null && offset_vec.Length == 1;
				
			m_values[progression_idx] = m_is_offset_from_last ? offset_vec[constant_offset ? 0 : progression_idx] : Vector3.zero;
			m_values[progression_idx] += new Vector3(m_from.x + (m_to.x - m_from.x) * UnityEngine.Random.value, m_from.y + (m_to.y - m_from.y) * UnityEngine.Random.value, m_from.z + (m_to.z - m_from.z) * UnityEngine.Random.value);
		}
		
		public void CalculateRotationProgressions(ref float[] letter_progressions, int num_progressions, Vector3[] offset_vecs, TextFxBezierCurve curve_override = null)
		{
			if(curve_override != null)
			{
				// Work out letter rotations based on the provided bezier curve setup
				
				bool constant_offset = offset_vecs != null && offset_vecs.Length == 1;
				m_values = new Vector3[num_progressions];
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					m_values[idx] = m_is_offset_from_last ? offset_vecs[constant_offset ? 0 : idx] : Vector3.zero;
				}
				
				for(int idx=0; idx < letter_progressions.Length; idx++)
				{
					m_values[idx] += curve_override.GetCurvePointRotation(letter_progressions[idx]);
				}
			}
			
			CalculateProgressions(num_progressions, curve_override == null ? offset_vecs : m_values, curve_override != null);
		}
		
		public virtual void CalculateProgressions(int num_progressions, Vector3[] offset_vecs, bool force_offset_from_last = false)
		{
			bool offset_from_last = force_offset_from_last ? true : m_is_offset_from_last;
			
			// Initialise the array of values. Array of only one if all progressions share the same constant value.
			if(Progression == (int) ValueProgression.Eased || Progression == (int) ValueProgression.EasedCustom || Progression == (int) ValueProgression.Random || (offset_from_last && offset_vecs.Length > 1))
			{
				bool constant_offset = offset_vecs != null && offset_vecs.Length == 1;
				m_values = new Vector3[num_progressions];
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					m_values[idx] = offset_from_last ? offset_vecs[constant_offset ? 0 : idx] : Vector3.zero;
				}
			}
			else
			{
				m_values = new Vector3[1]{offset_from_last && offset_vecs.Length >= 1 ? offset_vecs[0] : Vector3.zero};
			}
			
			if(Progression == (int) ValueProgression.Random)
			{
				for(int idx=0; idx < num_progressions; idx++)
				{
					m_values[idx] += new Vector3(m_from.x + (m_to.x - m_from.x) * UnityEngine.Random.value, m_from.y + (m_to.y - m_from.y) * UnityEngine.Random.value, m_from.z + (m_to.z - m_from.z) * UnityEngine.Random.value);
				}
			}
			else if(Progression == (int) ValueProgression.Eased)
			{
				float progression;
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					progression = num_progressions == 1 ? 0 : (float)idx / ((float)num_progressions - 1f);
					
					if(m_to_to_bool)
					{
						if(progression <= 0.5f)
						{
							m_values[idx] += m_from + (m_to - m_from) * EasingManager.GetEaseProgress(m_ease_type, progression/0.5f);
						}
						else
						{
							progression -= 0.5f;
							m_values[idx] += m_to + (m_to_to - m_to) * EasingManager.GetEaseProgress(EasingManager.GetEaseTypeOpposite(m_ease_type), progression/0.5f);
						}
					}
					else
					{
						m_values[idx] += m_from + (m_to - m_from) * EasingManager.GetEaseProgress(m_ease_type, progression);
					}
				}
				
			}
			else if(Progression == (int) ValueProgression.EasedCustom)
			{
				float progression;
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					progression = num_progressions == 1 ? 0 : (float)idx / ((float)num_progressions - 1f);
					
					if(m_ease_curve_per_axis)
					{
						m_values[idx].x += m_from.x + (m_to.x - m_from.x) * m_custom_ease_curve.Evaluate(progression);
						m_values[idx].y += m_from.y + (m_to.y - m_from.y) * m_custom_ease_curve_y.Evaluate(progression);
						m_values[idx].z += m_from.z + (m_to.z - m_from.z) * m_custom_ease_curve_z.Evaluate(progression);
					}
					else
						m_values[idx] += m_from + (m_to - m_from) * m_custom_ease_curve.Evaluate(progression);
				}
			}
			else if(Progression == (int) ValueProgression.Constant)
			{
				for(int idx=0; idx < m_values.Length; idx++)
				{
					m_values[idx] += m_from;
				}
			}
		}
		
		public ActionVector3Progression Clone()
		{
			ActionVector3Progression vector3_progression = new ActionVector3Progression(Vector3.zero);
			
			vector3_progression.m_progression_idx = Progression;
			vector3_progression.m_ease_type = m_ease_type;
			vector3_progression.m_from = m_from;
			vector3_progression.m_to = m_to;
			vector3_progression.m_to_to = m_to_to;
			vector3_progression.m_to_to_bool = m_to_to_bool;
			vector3_progression.m_is_offset_from_last = m_is_offset_from_last;
			vector3_progression.m_unique_randoms = m_unique_randoms;
			vector3_progression.m_override_animate_per_option = m_override_animate_per_option;
			vector3_progression.m_animate_per = m_animate_per;
			vector3_progression.m_ease_curve_per_axis = m_ease_curve_per_axis;
			vector3_progression.m_custom_ease_curve = new AnimationCurve(m_custom_ease_curve.keys);
			vector3_progression.m_custom_ease_curve_y = new AnimationCurve(m_custom_ease_curve_y.keys);
			vector3_progression.m_custom_ease_curve_z = new AnimationCurve(m_custom_ease_curve_z.keys);
			
			return vector3_progression;
		}

		public override JSONValue ExportData()
		{
			JSONObject json_data = new JSONObject();
			
			ExportBaseData(ref json_data);
			
			json_data["m_from"] = m_from.ExportData();
			json_data["m_to"] = m_to.ExportData();
			json_data["m_to_to"] = m_to_to.ExportData();
			json_data["m_ease_curve_per_axis"] = m_ease_curve_per_axis;
			
			if(Progression == (int) ValueProgression.EasedCustom && m_ease_curve_per_axis)
			{
				json_data["m_custom_ease_curve_y"] = m_custom_ease_curve_y.ExportData();
				json_data["m_custom_ease_curve_z"] = m_custom_ease_curve_z.ExportData();
			}
			
			return new JSONValue(json_data);
		}
		
		public override void ImportData(JSONObject json_data)
		{
			m_from = json_data["m_from"].Obj.JSONtoVector3();
			m_to = json_data["m_to"].Obj.JSONtoVector3();
			m_to_to = json_data["m_to_to"].Obj.JSONtoVector3();
			m_ease_curve_per_axis = json_data["m_ease_curve_per_axis"].Boolean;
			if(json_data.ContainsKey("m_custom_ease_curve_y"))
			{
				m_custom_ease_curve_y = json_data["m_custom_ease_curve_y"].Array.JSONtoAnimationCurve();
				m_custom_ease_curve_z = json_data["m_custom_ease_curve_z"].Array.JSONtoAnimationCurve();
			}
			
			ImportBaseData(json_data);
			
		}
		
		public void ImportLegacyData(string data_string)
		{
			KeyValuePair<string, string> value_pair;
			List<object> obj_list = data_string.StringToList(';',':');
			
			foreach(object obj in obj_list)
			{
				value_pair = (KeyValuePair<string, string>) obj;
				
				switch(value_pair.Key)
				{
					case "m_from": m_from = value_pair.Value.StringToVector3('|','<'); break;
					case "m_to": m_to = value_pair.Value.StringToVector3('|','<'); break;
					case "m_to_to": m_to_to = value_pair.Value.StringToVector3('|','<'); break;
					case "m_ease_curve_per_axis": m_ease_curve_per_axis = bool.Parse(value_pair.Value); break;
					case "m_custom_ease_curve_y": m_custom_ease_curve_y = value_pair.Value.ToAnimationCurve(); break;
					case "m_custom_ease_curve_z": m_custom_ease_curve_z = value_pair.Value.ToAnimationCurve(); break;
					
					default :
						ImportBaseLagacyData(value_pair); break;
				}
			}
		}
		
	#if UNITY_EDITOR
		public float DrawEditorGUI(GUIContent label, Rect position, bool offset_legal, bool unique_random_legal = false, bool bold_label = true)
		{
			float x_offset = position.x + ACTION_INDENT_LEVEL_1;
			float y_offset = DrawProgressionEditorHeader(label, position, offset_legal, unique_random_legal, bold_label, extra_options : ProgressionExtraOptions, extra_option_indexes : ProgressionExtraOptionIndexes);
			
			m_from = EditorGUI.Vector3Field(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), Progression == (int) ValueProgression.Constant ? "Vector" : "Vector From", m_from);
			y_offset += LINE_HEIGHT*2;
			
			if(Progression != (int) ValueProgression.Constant)
			{
				m_to = EditorGUI.Vector3Field(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Vector To", m_to);
				y_offset += LINE_HEIGHT*2;
				
				if(Progression == (int) ValueProgression.Eased && m_to_to_bool)
				{
					m_to_to = EditorGUI.Vector3Field(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Vector Then", m_to_to);
					y_offset += LINE_HEIGHT*2;
				}
				
				y_offset = DrawVector3CustomEaseCurveSettings(x_offset, y_offset);
			}
			
			return (y_offset) - position.y;
		}
		
		protected float DrawVector3CustomEaseCurveSettings(float x_offset, float y_offset)
		{
			if(Progression == (int) ValueProgression.EasedCustom)
			{
				EditorGUI.LabelField(new Rect(x_offset + VECTOR_3_WIDTH + 5, y_offset+1, 70, LINE_HEIGHT), new GUIContent("Per Axis?", "Enables the definition of a custom animation easing curve for each axis (x,y,z)."));
				m_ease_curve_per_axis = EditorGUI.Toggle(new Rect(x_offset + VECTOR_3_WIDTH + 75, y_offset, 20, LINE_HEIGHT), m_ease_curve_per_axis);
				m_custom_ease_curve = EditorGUI.CurveField(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Ease Curve" + (m_ease_curve_per_axis ? " (x)" : ""), m_custom_ease_curve );
				y_offset += LINE_HEIGHT * 1.2f;
				
				if(m_ease_curve_per_axis)
				{
					m_custom_ease_curve_y = EditorGUI.CurveField(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Ease Curve (y)", m_custom_ease_curve_y );
					y_offset += LINE_HEIGHT * 1.2f;
					m_custom_ease_curve_z = EditorGUI.CurveField(new Rect(x_offset, y_offset, VECTOR_3_WIDTH, LINE_HEIGHT), "Ease Curve (z)", m_custom_ease_curve_z );
					y_offset += LINE_HEIGHT * 1.2f;
				}
			}
			
			return y_offset;
		}
	#endif
	}


	[System.Serializable]
	public class ActionColorProgression : ActionVariableProgression
	{
		[SerializeField]
		Color[] m_values;
		[SerializeField]
		Color m_from = Color.white;
		[SerializeField]
		Color m_to = Color.white;
		[SerializeField]
		Color m_to_to = Color.white;
		
		public Color ValueFrom { get { return m_from; } }
		public Color ValueTo { get { return m_to; } }
		public Color ValueThen { get { return m_to_to; } }
		public Color[] Values { get { return m_values; } set { m_values = value; } }
		
		public void SetConstant( Color constant_value )
		{
			m_progression_idx = (int) ValueProgression.Constant;
			m_from = constant_value;
		}
		
		public void SetRandom( Color random_min, Color random_max, bool unique_randoms = false)
		{
			m_progression_idx = (int) ValueProgression.Random;
			m_from = random_min;
			m_to = random_max;
			m_unique_randoms = unique_randoms;
		}
		
		public void SetEased( EasingEquation easing_function, Color eased_from, Color eased_to)
		{
			m_progression_idx = (int) ValueProgression.Eased;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to_bool = false;
			m_ease_type = easing_function;
		}
		
		public void SetEased( EasingEquation easing_function, Color eased_from, Color eased_to, Color eased_then)
		{
			m_progression_idx = (int) ValueProgression.Eased;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to = eased_then;
			m_to_to_bool = true;
			m_ease_type = easing_function;
		}
		
		public void SetEasedCustom ( AnimationCurve easing_curve, Color eased_from, Color eased_to)
		{
			m_progression_idx = (int) ValueProgression.EasedCustom;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to_bool = false;
			
			m_custom_ease_curve = easing_curve;
		}
		
		public Color GetValue(AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per_default)
		{
			return GetValue(GetProgressionIndex(progression_variables,animate_per_default));
		}
		
		public Color GetValue(int progression_idx)
		{
			int num_vals = m_values.Length;
			if(num_vals > 1 && progression_idx < num_vals)
			{
				return m_values[progression_idx];
			}
			else if(num_vals==1)
			{
				return m_values[0];
			}
			else
			{
				return Color.white;
			}
		}
		
	#if UNITY_EDITOR
		public int NumEditorLines
		{
			get
			{
				return Progression == (int) ValueProgression.Constant ? 2 : 3;
			}
		}
	#endif
		
		public ActionColorProgression(Color start_colour)
		{
			m_from = start_colour;
			m_to = start_colour;
			m_to_to = start_colour;
		}
		
		public void CalculateUniqueRandom(AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per, Color[] offset_cols)
		{
			int progression_idx = GetProgressionIndex(progression_variables, animate_per);
			bool constant_offset = offset_cols != null && offset_cols.Length == 1;
				
			m_values[progression_idx] = m_is_offset_from_last ? offset_cols[constant_offset ? 0 : progression_idx] : new Color(0,0,0,0);
			m_values[progression_idx] += m_from + (m_to - m_from) * UnityEngine.Random.value;
		}
		
		public void CalculateProgressions(int num_progressions, Color[] offset_cols)
		{
			
			if(Progression == (int) ValueProgression.Eased || Progression == (int) ValueProgression.EasedCustom || Progression == (int) ValueProgression.Random || (m_is_offset_from_last && offset_cols.Length > 1))
			{
				bool constant_offset = offset_cols != null && offset_cols.Length == 1;
				m_values = new Color[num_progressions];
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					m_values[idx] = m_is_offset_from_last ? offset_cols[constant_offset ? 0 : idx] : new Color(0,0,0,0);
				}
			}
			else
			{
				m_values = new Color[1]{ m_is_offset_from_last ? offset_cols[0] : new Color(0,0,0,0) };
			}
			
			if(Progression == (int) ValueProgression.Random) // && (progression >= 0 || m_unique_randoms))
			{
				for(int idx=0; idx < num_progressions; idx++)
				{
					m_values[idx] += m_from + (m_to - m_from) * UnityEngine.Random.value;
				}
			}
			else if(Progression == (int) ValueProgression.Eased)
			{
				float progression;
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					progression = num_progressions == 1 ? 0 : (float)idx / ((float)num_progressions - 1f);
					
					if(m_to_to_bool)
					{
						if(progression  <= 0.5f)
						{
							m_values[idx] += m_from + (m_to - m_from) * EasingManager.GetEaseProgress(m_ease_type, progression/0.5f);
						}
						else
						{
							progression -= 0.5f;
							m_values[idx] += m_to + (m_to_to - m_to) * EasingManager.GetEaseProgress(EasingManager.GetEaseTypeOpposite(m_ease_type), progression/0.5f);
						}
					}
					else
					{
						m_values[idx] += m_from + (m_to - m_from) * EasingManager.GetEaseProgress(m_ease_type, progression);
					}
				}
			}
			else if(Progression == (int) ValueProgression.EasedCustom)
			{
				float progression;
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					progression = num_progressions == 1 ? 0 : (float)idx / ((float)num_progressions - 1f);
					
					m_values[idx] += m_from + (m_to - m_from) * m_custom_ease_curve.Evaluate(progression);
				}
			}
			else if(Progression == (int) ValueProgression.Constant)
			{
				for(int idx=0; idx < m_values.Length; idx++)
				{
					m_values[idx] += m_from;
				}
			}
		}
		
		public ActionColorProgression Clone()
		{
			ActionColorProgression color_progression = new ActionColorProgression(Color.white);
			
			color_progression.m_progression_idx = Progression;
			color_progression.m_ease_type = m_ease_type;
			color_progression.m_from = m_from;
			color_progression.m_to = m_to;
			color_progression.m_to_to = m_to_to;
			color_progression.m_to_to_bool = m_to_to_bool;
			color_progression.m_is_offset_from_last = m_is_offset_from_last;
			color_progression.m_unique_randoms = m_unique_randoms;
			color_progression.m_override_animate_per_option = m_override_animate_per_option;
			color_progression.m_animate_per = m_animate_per;
			
			return color_progression;
		}

		public override JSONValue ExportData()
		{
			JSONObject json_data = new JSONObject();
			
			ExportBaseData(ref json_data);
			
			json_data["m_from"] = m_from.ExportData();
			json_data["m_to"] = m_to.ExportData();
			json_data["m_to_to"] = m_to_to.ExportData();
			
			return new JSONValue(json_data);
		}
		
		public override void ImportData(JSONObject json_data)
		{
			m_from = json_data["m_from"].Obj.JSONtoColor();
			m_to = json_data["m_to"].Obj.JSONtoColor();
			m_to_to = json_data["m_to_to"].Obj.JSONtoColor();
			
			ImportBaseData(json_data);
		}
		
		public void ImportLegacyData(string data_string)
		{
			KeyValuePair<string, string> value_pair;
			List<object> obj_list = data_string.StringToList(';',':');
			
			foreach(object obj in obj_list)
			{
				value_pair = (KeyValuePair<string, string>) obj;
				
				switch(value_pair.Key)
				{
				case "m_from": m_from = value_pair.Value.StringToColor('|','<'); break;
				case "m_to": m_to = value_pair.Value.StringToColor('|','<'); break;
				case "m_to_to": m_to_to = value_pair.Value.StringToColor('|','<'); break;
					
				default :
					ImportBaseLagacyData(value_pair); break;
				}
			}
		}
		
	#if UNITY_EDITOR
		public float DrawEditorGUI(GUIContent label, Rect position, bool offset_legal, bool unique_random_legal = false, bool bold_label = true)
		{
			float x_offset = position.x + ACTION_INDENT_LEVEL_1;
			float y_offset = DrawProgressionEditorHeader(label, position, offset_legal, unique_random_legal, bold_label, extra_options : ProgressionExtraOptions, extra_option_indexes : ProgressionExtraOptionIndexes);
			
			EditorGUI.LabelField(new Rect(x_offset, y_offset, 50, LINE_HEIGHT * 2), Progression == (int) ValueProgression.Constant ? "Colour" : "Colour\nFrom", EditorStyles.miniLabel);
			x_offset += 60;
			
			m_from = EditorGUI.ColorField(new Rect(x_offset, y_offset, LINE_HEIGHT*2, LINE_HEIGHT), m_from);
			
			if(Progression != (int) ValueProgression.Constant)
			{
				x_offset += 65;
				
				EditorGUI.LabelField(new Rect(x_offset, y_offset, 50, LINE_HEIGHT*2), "Colour\nTo", EditorStyles.miniBoldLabel);
				x_offset += 60;
				
				m_to = EditorGUI.ColorField(new Rect(x_offset, y_offset, LINE_HEIGHT*2, LINE_HEIGHT), m_to);
				
				if(Progression == (int) ValueProgression.Eased && m_to_to_bool)
				{
					x_offset += 65;
				
					EditorGUI.LabelField(new Rect(x_offset, y_offset, 50, LINE_HEIGHT*2), "Colour\nThen To", EditorStyles.miniBoldLabel);
					x_offset += 60;
					
					m_to_to = EditorGUI.ColorField(new Rect(x_offset, y_offset, LINE_HEIGHT*2, LINE_HEIGHT), m_to_to);
				}
				
				if(Progression == (int) ValueProgression.EasedCustom)
				{
					m_custom_ease_curve = EditorGUI.CurveField(new Rect(position.x + ACTION_INDENT_LEVEL_1, y_offset + LINE_HEIGHT + 10, VECTOR_3_WIDTH, LINE_HEIGHT), "Ease Curve", m_custom_ease_curve );
					y_offset += LINE_HEIGHT * 1.2f;
				}
			}
			
			return (y_offset + LINE_HEIGHT + 10) - position.y;
		}
	#endif
	}

	[System.Serializable]
	public class ActionVertexColorProgression : ActionVariableProgression
	{
		[SerializeField]
		VertexColour[] m_values;
		[SerializeField]
		VertexColour m_from = new VertexColour();
		[SerializeField]
		VertexColour m_to = new VertexColour();
		[SerializeField]
		VertexColour m_to_to = new VertexColour();
		
		public VertexColour ValueFrom { get { return m_from; } }
		public VertexColour ValueTo { get { return m_to; } }
		public VertexColour ValueThen { get { return m_to_to; } }
		public VertexColour[] Values { get { return m_values; } set { m_values = value; } }
		
		public void SetConstant( VertexColour constant_value )
		{
			m_progression_idx = (int) ValueProgression.Constant;
			m_from = constant_value;
		}
		
		public void SetRandom( VertexColour random_min, VertexColour random_max, bool unique_randoms = false)
		{
			m_progression_idx = (int) ValueProgression.Random;
			m_from = random_min;
			m_to = random_max;
			m_unique_randoms = unique_randoms;
		}
		
		public void SetEased( EasingEquation easing_function, VertexColour eased_from, VertexColour eased_to)
		{
			m_progression_idx = (int) ValueProgression.Eased;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to_bool = false;
			m_ease_type = easing_function;
		}
		
		public void SetEased( EasingEquation easing_function, VertexColour eased_from, VertexColour eased_to, VertexColour eased_then)
		{
			m_progression_idx = (int) ValueProgression.Eased;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to = eased_then;
			m_to_to_bool = true;
			m_ease_type = easing_function;
		}
		
		public void SetEasedCustom ( AnimationCurve easing_curve, VertexColour eased_from, VertexColour eased_to)
		{
			m_progression_idx = (int) ValueProgression.EasedCustom;
			m_from = eased_from;
			m_to = eased_to;
			m_to_to_bool = false;
			
			m_custom_ease_curve = easing_curve;
		}
		
		public VertexColour GetValue(AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per_default)
		{
			return GetValue(GetProgressionIndex(progression_variables,animate_per_default));
		}
		
		public VertexColour GetValue(int progression_idx)
		{
			int num_vals = m_values.Length;
			if(num_vals > 1 && progression_idx < num_vals)
			{
				return m_values[progression_idx];
			}
			else if(num_vals==1)
			{
				return m_values[0];
			}
			else
			{
				return new VertexColour(Color.white);
			}
		}
		
	#if UNITY_EDITOR
		public int NumEditorLines
		{
			get
			{
				return Progression == (int) ValueProgression.Constant ? 3 : 4;
			}
		}
	#endif
		
		public ActionVertexColorProgression(VertexColour start_colour)
		{
			m_from = start_colour.Clone();
			m_to = start_colour.Clone();
			m_to_to = start_colour.Clone();
		}
		
		public void ConvertFromFlatColourProg(ActionColorProgression flat_colour_progression)
		{
			m_progression_idx = flat_colour_progression.Progression;
			m_ease_type = flat_colour_progression.EaseType;
			m_from = new VertexColour(flat_colour_progression.ValueFrom);
			m_to = new VertexColour(flat_colour_progression.ValueTo);
			m_to_to = new VertexColour(flat_colour_progression.ValueThen);
			m_to_to_bool = flat_colour_progression.UsingThirdValue;
			m_is_offset_from_last = flat_colour_progression.IsOffsetFromLast;
			m_unique_randoms = flat_colour_progression.UniqueRandom;
		}
		
		public void CalculateUniqueRandom(AnimationProgressionVariables progression_variables, AnimatePerOptions animate_per, VertexColour[] offset_colours)
		{
			int progression_idx = GetProgressionIndex(progression_variables, animate_per);
			bool constant_offset = offset_colours != null && offset_colours.Length == 1;
				
			m_values[progression_idx] = m_is_offset_from_last ? offset_colours[constant_offset ? 0 : progression_idx].Clone() : new VertexColour(new Color(0,0,0,0));
			m_values[progression_idx] = m_values[progression_idx].Add(m_from.Add(m_to.Sub(m_from).Multiply(UnityEngine.Random.value)));
		}
		
		public void CalculateProgressions(int num_progressions, VertexColour[] offset_vert_colours, Color[] offset_colours)
		{
			if(Progression == (int) ValueProgression.Eased || Progression == (int) ValueProgression.EasedCustom || Progression == (int) ValueProgression.Random || (m_is_offset_from_last && ((offset_colours != null && offset_colours.Length > 1) || (offset_vert_colours != null && offset_vert_colours.Length > 1) )))
			{
				bool constant_offset = (offset_colours != null && offset_colours.Length == 1) || (offset_vert_colours != null && offset_vert_colours.Length == 1);
				m_values = new VertexColour[num_progressions];
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					m_values[idx] = m_is_offset_from_last ? 
										(offset_colours != null ? new VertexColour(offset_colours[constant_offset ? 0 : idx]) : offset_vert_colours[constant_offset ? 0 : idx].Clone())
							
										: new VertexColour(new Color(0,0,0,0));
				}
			}
			else
			{
				m_values = new VertexColour[1]{ m_is_offset_from_last ? 
										(offset_colours != null ? new VertexColour(offset_colours[0]) : offset_vert_colours[0].Clone())
							
										: new VertexColour(new Color(0,0,0,0)) };
			}
			
			
			if(Progression == (int) ValueProgression.Random)
			{
				for(int idx=0; idx < num_progressions; idx++)
				{
					m_values[idx] = m_values[idx].Add(m_from.Add(m_to.Sub(m_from).Multiply(UnityEngine.Random.value)));
				}
			}
			else if(Progression == (int) ValueProgression.Eased)
			{
				float progression;
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					progression = num_progressions == 1 ? 0 : (float)idx / ((float)num_progressions - 1f);
				
					if(m_to_to_bool)
					{
						if(progression  <= 0.5f)
						{
							m_values[idx] = m_values[idx].Add(m_from.Add((m_to.Sub(m_from)).Multiply(EasingManager.GetEaseProgress(m_ease_type, progression/0.5f))));
						}
						else
						{
							progression -= 0.5f;
							m_values[idx] = m_values[idx].Add(m_to.Add((m_to_to.Sub(m_to)).Multiply(EasingManager.GetEaseProgress(m_ease_type, progression/0.5f))));
						}
					}
					else
					{
						m_values[idx] = m_values[idx].Add(m_from.Add((m_to.Sub(m_from)).Multiply(EasingManager.GetEaseProgress(m_ease_type, progression))));
					}
				}
			}
			else if(Progression == (int) ValueProgression.EasedCustom)
			{
				float progression;
				
				for(int idx=0; idx < num_progressions; idx++)
				{
					progression = num_progressions == 1 ? 0 : (float)idx / ((float)num_progressions - 1f);
					
					m_values[idx] = m_values[idx].Add(m_from.Add((m_to.Sub(m_from)).Multiply(m_custom_ease_curve.Evaluate(progression))));
				}
			}
			else if(Progression == (int) ValueProgression.Constant)
			{
				for(int idx=0; idx < m_values.Length; idx++)
				{
					m_values[idx] = m_values[idx].Add(m_from);
				}
			}
		}
		
		public ActionVertexColorProgression Clone()
		{
			ActionVertexColorProgression color_progression = new ActionVertexColorProgression(new VertexColour());
			
			color_progression.m_progression_idx = Progression;
			color_progression.m_ease_type = m_ease_type;
			color_progression.m_from = m_from.Clone();
			color_progression.m_to = m_to.Clone();
			color_progression.m_to_to = m_to_to.Clone();
			color_progression.m_to_to_bool = m_to_to_bool;
			color_progression.m_is_offset_from_last = m_is_offset_from_last;
			color_progression.m_unique_randoms = m_unique_randoms;
			color_progression.m_override_animate_per_option = m_override_animate_per_option;
			color_progression.m_animate_per = m_animate_per;
			
			return color_progression;
		}

		public override JSONValue ExportData()
		{
			JSONObject json_data = new JSONObject();
			
			ExportBaseData(ref json_data);
			
			json_data["m_from"] = m_from.ExportData();
			json_data["m_to"] = m_to.ExportData();
			json_data["m_to_to"] = m_to_to.ExportData();
			
			return new JSONValue(json_data);
		}
		
		public override void ImportData(JSONObject json_data)
		{
			m_from = json_data["m_from"].Obj.JSONtoVertexColour();
			m_to = json_data["m_to"].Obj.JSONtoVertexColour();
			m_to_to = json_data["m_to_to"].Obj.JSONtoVertexColour();
			
			ImportBaseData(json_data);
		}
		
		public void ImportLegacyData(string data_string)
		{
			KeyValuePair<string, string> value_pair;
			List<object> obj_list = data_string.StringToList(';',':');
			
			foreach(object obj in obj_list)
			{
				value_pair = (KeyValuePair<string, string>) obj;
				
				switch(value_pair.Key)
				{
				case "m_from": m_from = value_pair.Value.StringToVertexColor('|','<','^'); break;
				case "m_to": m_to = value_pair.Value.StringToVertexColor('|','<','^'); break;
				case "m_to_to": m_to_to = value_pair.Value.StringToVertexColor('|','<','^'); break;
					
				default :
					ImportBaseLagacyData(value_pair); break;
				}
			}
		}
		
	#if UNITY_EDITOR
		public float DrawEditorGUI(GUIContent label, Rect position, bool offset_legal, bool unique_random_legal = false, bool bold_label = true)
		{
			float y_offset = DrawProgressionEditorHeader(label, position, offset_legal, unique_random_legal, bold_label, extra_options : ProgressionExtraOptions, extra_option_indexes : ProgressionExtraOptionIndexes);
			float x_offset = position.x + ACTION_INDENT_LEVEL_1;
			
			EditorGUI.LabelField(new Rect(x_offset, y_offset, 50, LINE_HEIGHT * 2), Progression == (int) ValueProgression.Constant ? "Colours" : "Colours\nFrom", EditorStyles.miniBoldLabel);
			x_offset += 60;
			
			m_from.top_left = EditorGUI.ColorField(new Rect(x_offset, y_offset, LINE_HEIGHT*2, LINE_HEIGHT), m_from.top_left);
			m_from.bottom_left = EditorGUI.ColorField(new Rect(x_offset, y_offset + LINE_HEIGHT, LINE_HEIGHT*2, LINE_HEIGHT), m_from.bottom_left);
			x_offset += 45;
			m_from.top_right = EditorGUI.ColorField(new Rect(x_offset, y_offset, LINE_HEIGHT*2, LINE_HEIGHT), m_from.top_right);
			m_from.bottom_right = EditorGUI.ColorField(new Rect(x_offset, y_offset + LINE_HEIGHT, LINE_HEIGHT*2, LINE_HEIGHT), m_from.bottom_right);
			
			
			if(Progression != (int) ValueProgression.Constant)
			{
				x_offset += 65;
				
				EditorGUI.LabelField(new Rect(x_offset, y_offset, 50, LINE_HEIGHT*2), "Colours\nTo", EditorStyles.miniBoldLabel);
				x_offset += 60;
				
				m_to.top_left = EditorGUI.ColorField(new Rect(x_offset, y_offset, LINE_HEIGHT*2, LINE_HEIGHT), m_to.top_left);
				m_to.bottom_left = EditorGUI.ColorField(new Rect(x_offset, y_offset + LINE_HEIGHT, LINE_HEIGHT*2, LINE_HEIGHT), m_to.bottom_left);
				x_offset += 45;
				m_to.top_right = EditorGUI.ColorField(new Rect(x_offset, y_offset, LINE_HEIGHT*2, LINE_HEIGHT), m_to.top_right);
				m_to.bottom_right = EditorGUI.ColorField(new Rect(x_offset, y_offset + LINE_HEIGHT, LINE_HEIGHT*2, LINE_HEIGHT), m_to.bottom_right);
				
				
				if(Progression == (int) ValueProgression.Eased && m_to_to_bool)
				{
					x_offset += 65;
				
					EditorGUI.LabelField(new Rect(x_offset, y_offset, 50, LINE_HEIGHT*2), "Colours\nThen To", EditorStyles.miniBoldLabel);
					x_offset += 60;
					
					m_to_to.top_left = EditorGUI.ColorField(new Rect(x_offset, y_offset, LINE_HEIGHT*2, LINE_HEIGHT), m_to_to.top_left);
					m_to_to.bottom_left = EditorGUI.ColorField(new Rect(x_offset, y_offset + LINE_HEIGHT, LINE_HEIGHT*2, LINE_HEIGHT), m_to_to.bottom_left);
					x_offset += 45;
					m_to_to.top_right = EditorGUI.ColorField(new Rect(x_offset, y_offset, LINE_HEIGHT*2, LINE_HEIGHT), m_to_to.top_right);
					m_to_to.bottom_right = EditorGUI.ColorField(new Rect(x_offset, y_offset + LINE_HEIGHT, LINE_HEIGHT*2, LINE_HEIGHT), m_to_to.bottom_right);
				}
				
				if(Progression == (int) ValueProgression.EasedCustom)
				{
					m_custom_ease_curve = EditorGUI.CurveField(new Rect(position.x + ACTION_INDENT_LEVEL_1, y_offset + LINE_HEIGHT * 2 + 10, VECTOR_3_WIDTH, LINE_HEIGHT), "Ease Curve", m_custom_ease_curve );
					y_offset += LINE_HEIGHT * 1.2f;
				}
			}
			
			return (y_offset + LINE_HEIGHT * 2 + 10) - position.y;
		}
	#endif
	}
}