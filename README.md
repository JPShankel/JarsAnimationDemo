**Jars Animation Demo Implementation Notes**

The animation demo is implemented in Unity 6. The main entry point is in Assets/SampleScene. The three areas of work are the AnimationCharacter, the AnimationNavigator and the panels in the Canvas.

**The Character**
The AnimationCharacter is imported from the provided assets. An Animator component has been added with a single transition from the Entry state to a Blend Tree.

The Blend Tree node arranges the six input animations in a freeform Cartesian space with 2 parameters (BlendX and BlendY). The animations are laid out in a circular pattern to minimize crosstalk in animation blending. Animation transitions are performed in the AnimatorNavigator script by setting the BlendX and BlendY parameters on the Animator.

**The UI** 
The Canvas defines 3 panels to manage the UI. There's a background panel with static text titles, a Category Panel, which defines the space for the animation category tabs and a Dance Panel which defines the space for the specific animation buttons.

The two button panels use a Vertical and Horizontal Layout Group (respectively) to control positioning of button elements. The buttons are dynamically generated in the AnimationNavigator script. The Panels also define Toggle Groups for their buttons which ensure that only one button in each group will be selected at a time.

Note: when using only one toggle group, the system will keep the currently selected button highlighted automatically if "Deselect On Background Click" is turned off in the Event System. Unfortunately, adding a second toggle group breaks this. Since we never have to worry about graying these buttons out, a simple solution is to make a button non-interactable if it has been marked with 'isOn' by the group. The groups successfully maintain the internal state of the button, it's just the graphical state that doesn't work. So the 'Disabled Color' is set to a highlighted value in the button prefabs and a script was added that sets interactable to !isOn. 

**The Navigator** 
The bulk of the logic for playing and selecting animations is in AnimationNavigator.cs. 

In Start(), we set up a map of animation categories to specific animations. The three categories I chose are 'Dancing' for the two dance anims, 'Jumping' for the two jump anims and 'Acting' for idle and dying. Because dying is easy and comedy is hard :)

We also set up a map of specific animation names to blend tree parameters and add category buttons (based on the initialized map) to the category panel in the canvas.

When a category is selected, we build a list of animation buttons and add them to the animation panel. Animation timings are read from the animation clips in the character animation controller and stored in a map by name.

When a new animation is selected, we look up its blend parameters and interpolate toward them. Setting blend parameters directly is not smooth, so we track the set of target parameters that are interpolated every frame.

**Things To Add** 

Given more time, I would really like to make the coordination between animation names, categories, timings and blend parameters more data driven. It would be nice to have a separate resource that reads this from a database that can be edited easily.

I'd like to expose the animation blending parameter to let users select transition smoothness. It would also be nice to group animations in the Cartesian space based on how well they blend and to make sure that only the source and destination animations participate in blending.

The Toggle Group system is convenient for persisting UI state, but it's very annoying that the visual state has to be manually reconciled to the selection state. Using the interactable property is adequate for the immediate UX but it would not be a good long term solution. It would be worth some time to subclass the Toggle button to be aware of group state and behave more intuitively. Swapping the entire color scheme when isOn so that highlighting and selection will still behave would be a good solution.

I'd like to add more camera and playback controls. Pause, restart, reverse and playback speed would be nice. I'd also like a simple "theta/phi/zoom" radial camera so we can see the character from various angles and distances.
