using UnityEngine;
using UnityEngine.Events;

public class OnEvent : MonoBehaviour
{
	[SerializeField] private EventTrigger eventStartTrigger = EventTrigger.None;
    [SerializeField] [Tag] private string collisionTag = null;
	[SerializeField] private bool triggerOnce = false;
	[SerializeField] private UnityEvent unityEvent = new UnityEvent();

	public enum EventTrigger
	{
		None,
		OnStart,
		OnDestroy,
		OnEnable,
		OnDisable,
		TriggerEnter,
		TriggerExit,
		TriggerEnter2D,
		TriggerExit2D,
		CollisionEnter,
		CollisionExit,
		CollisionEnter2D,
		CollisionExit2D,
		MouseEnter,
		MouseExit,
		MouseDown,
		MouseUp,
	}

    private bool CheckTag(string tag)
    {
        return collisionTag == "Untagged" || collisionTag == tag;
    }

	private void Start()
	{
		HandleGameEvent(EventTrigger.OnStart);
	}

	private void OnDestroy()
	{
		HandleGameEvent(EventTrigger.OnDestroy);
	}

	private void OnEnable()
	{
		HandleGameEvent(EventTrigger.OnEnable);
	}

	private void OnDisable()
	{
		HandleGameEvent(EventTrigger.OnDisable);
	}

	private void OnTriggerEnter(Collider other)
	{
        if (CheckTag(other.tag))
        {
            HandleGameEvent(EventTrigger.TriggerEnter);
        }
    }

	private void OnTriggerExit(Collider other)
	{
		if (CheckTag(other.tag))
		{
			HandleGameEvent(EventTrigger.TriggerExit);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (CheckTag(other.tag))
		{
			HandleGameEvent(EventTrigger.TriggerEnter2D);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (CheckTag(other.tag))
		{
			HandleGameEvent(EventTrigger.TriggerExit2D);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (CheckTag(collision.transform.tag))
		{
			HandleGameEvent(EventTrigger.CollisionEnter);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (CheckTag(collision.transform.tag))
		{
			HandleGameEvent(EventTrigger.CollisionExit);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (CheckTag(collision.transform.tag))
		{
			HandleGameEvent(EventTrigger.CollisionEnter2D);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (CheckTag(collision.transform.tag))
		{
			HandleGameEvent(EventTrigger.CollisionExit2D);
		}
	}

	private void OnMouseEnter()
	{
		HandleGameEvent(EventTrigger.MouseEnter);
	}

	private void OnMouseExit()
	{
		HandleGameEvent(EventTrigger.MouseExit);
	}

	private void OnMouseDown()
	{
		HandleGameEvent(EventTrigger.MouseDown);
	}

	private void OnMouseUp()
	{
		HandleGameEvent(EventTrigger.MouseUp);
	}

	private void HandleGameEvent(EventTrigger gameEvent)
	{
		if (eventStartTrigger == gameEvent)
		{
			Play();
		}
	}

	private void Play()
	{
		unityEvent.Invoke();
		if (triggerOnce && eventStartTrigger != EventTrigger.OnDestroy)
			Destroy(this);
	}
}
