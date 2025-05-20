using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Collections.Specialized;
using TMPro;

public class AnimationNavigator : MonoBehaviour
{
    private Animator characterAnimator;

    private Dictionary<string, Vector2> animBlendParams;
    private Dictionary<string, float> animationTimings;
    private Dictionary<string, string[]> animCategories;
    private Vector2 currentBlendParams, targetBlendParams;

    public GameObject AnimationButtonPrefab;
    public GameObject CategoryButtonPrefab;

    List<GameObject> animationButtons;

    void Start()
    {
        /*
         * AnimationCharacter's Animator uses a blend tree with two
         * Cartesian parameters laid out in a hexagon. 
         */
        animBlendParams = new Dictionary<string, Vector2>();
        animBlendParams.Add("Dying", new Vector2(-0.25f, 0.25f));
        animBlendParams.Add("Joyful Jump", new Vector2(-1.0f, 0.0f));
        animBlendParams.Add("Jump Push Up", new Vector2(-1.75f, 1.0f));
        animBlendParams.Add("Old Man Idle", new Vector2(-0.25f, 1.0f));
        animBlendParams.Add("Silly Dancing", new Vector2(-1.0f, 1.5f));
        animBlendParams.Add("Twist Dance", new Vector2(-1.75f, 0.25f));

        // Start off in the center of the paramter space
        currentBlendParams = targetBlendParams = new Vector2(0,0);


        /* 
         * Build the category tab buttons
         */
        animCategories = new Dictionary<string, string[]>();
        animCategories.Add("Dancing", new[] { "Silly Dancing", "Twist Dance" });
        animCategories.Add("Jumping", new[] { "Joyful Jump", "Jump Push Up" });
        animCategories.Add("Acting", new[] { "Old Man Idle", "Dying" });

        GameObject panel = GameObject.Find("Category Panel");

        /*
        * Use a toggle group to maintain radio state
        * The toggle group system keeps track of selected button but
        * loses graphical quality, so there's a script added to the prefabs
        * that makes a button non-interactable when its selected, because
        * that state is preserved. A more robust solution would manage the
        * color palette based on isOn state, but since these buttons are
        * never grayed out the interactability scheme is simple and works
        */ 
        ToggleGroup group = panel.GetComponent<ToggleGroup>();

		if (panel == null)
		{
			UnityEngine.Debug.LogError("Could not find Category Panel");
			return;
		}

		if (group == null)
		{
			UnityEngine.Debug.LogError("Could not find Category Panel toggle group");
			return;
		}

		foreach (KeyValuePair<string, string[]> kvp in animCategories)
        {
            GameObject buttonOb = GameObject.Instantiate(CategoryButtonPrefab);
            buttonOb.transform.SetParent(panel.transform, false);
            buttonOb.GetComponent<Toggle>().onValueChanged.AddListener(delegate { SelectCategory(kvp.Key); });
            buttonOb.GetComponent<Toggle>().group = group;
            buttonOb.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = kvp.Key;
        }

		GameObject animationCharacter = GameObject.Find("AnimationCharacter");
		characterAnimator = animationCharacter ? animationCharacter.GetComponent<Animator>() : null;

		if (characterAnimator == null)
		{
			UnityEngine.Debug.LogError("Could not load animation character");
            return;
		}

		animationTimings = new Dictionary<string, float>();
        foreach (AnimationClip anim in characterAnimator.runtimeAnimatorController.animationClips)
        {
            animationTimings[anim.name] = anim.length;
        }

		animationButtons = new List<GameObject>();
		SelectCategory("Dancing");
	}

    void Update()
    {
        if (characterAnimator != null)
        {
            /*
             * Snapping to the selected anim's blend parameters creates abrupt transitions.
             * Interpolating from the current value to the target value is smoother
             */
            currentBlendParams = Vector2.Lerp(currentBlendParams, targetBlendParams, 0.05f);
            characterAnimator.SetFloat("BlendX", currentBlendParams.x);
            characterAnimator.SetFloat("BlendY", currentBlendParams.y);
        }
        else
        {
			UnityEngine.Debug.LogError("Attempting to acquire animation character in Update");
			GameObject animationCharacter = GameObject.Find("AnimationCharacter");
			characterAnimator = animationCharacter ? animationCharacter.GetComponent<Animator>() : null;
		}
	}

    void BuildAnimationButtons(string[] category)
    {
        foreach (GameObject ob in animationButtons)
        {
            GameObject.Destroy(ob);
        }
        animationButtons.Clear();

        /*
         * As with the category buttons, we use a toggle group to maintain radio
         * state among the animation buttons. This prefab also uses the interactable
         * trick to maintain visual feedback of selected buttons
         */
        GameObject panel = GameObject.Find("Dance Panel");
        ToggleGroup group = panel ? panel.GetComponent<ToggleGroup>() : null;

        if (panel == null)
        {
            UnityEngine.Debug.LogError("Could not find Dance Panel");
            return;
        }

		if (group == null)
		{
			UnityEngine.Debug.LogError("Could not find Dance Panel toggle group");
			return;
		}

		foreach (string anim in category)
        {
            GameObject buttonOb = GameObject.Instantiate(AnimationButtonPrefab);
            buttonOb.transform.SetParent(panel.transform, false);
            buttonOb.GetComponent<Toggle>().onValueChanged.AddListener(delegate { SelectAnimation(anim); });
            buttonOb.GetComponent<Toggle>().group = group;
			buttonOb.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = anim;

            float length;
            if (animationTimings.TryGetValue(anim,out length))
            {
                buttonOb.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = string.Format("Length: {0} seconds", length.ToString("0.00"));
			}
			animationButtons.Add(buttonOb);
        }
    }

    public void SelectCategory(string category)
    {
		string[] anims;
		if (animCategories.TryGetValue(category, out anims))
		{
			BuildAnimationButtons(anims);
		}
	}

	public void SelectAnimation(string anim)
    {
        Vector2 blendParams;
        if (characterAnimator && animBlendParams.TryGetValue(anim, out blendParams))
        {
            targetBlendParams = blendParams;
		}
	}
}
