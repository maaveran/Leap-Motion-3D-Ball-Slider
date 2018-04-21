using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Create a module that every tick sends a 'Move' event to
 * the target object
 */
public class MyInputModule : BaseInputModule
{
	public GameObject m_TargetObject;
	
	public override void Process()
	{
		if (m_TargetObject == null)
			return;
		ExecuteEvents.Execute (m_TargetObject, new BaseEventData (eventSystem), ExecuteEvents.moveHandler);
	}
}