using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeedbackPlayer : MonoBehaviour
{
    private List<Feedback> _feedbacks;
    
    private void Awake()
    {
        _feedbacks = GetComponentsInChildren<Feedback>().ToList();
    }

    public void PlayFeedbacks()
    {
        foreach (var feedback in _feedbacks)
            feedback.PlayFeedback();
    }
}
