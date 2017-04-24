using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
	public void LoadMenu ()
	{
		StaticLoadMenu ();
	}

	public void LoadTutorial ()
	{
		StaticLoadTutorial ();
	}

	public void LoadPlay ()
	{
		StaticLoadPlay ();	
	}

	public static void StaticLoadMenu ()
	{
		SceneManager.LoadScene ("Scenes/Menu/MenuScene");
	}

	public static void StaticLoadTutorial ()
	{
		SceneManager.LoadScene ("Scenes/Tutorial/TutorialScene");
	}

	public static void StaticLoadPlay ()
	{
		SceneManager.LoadScene ("Scenes/Tree/TreeScene");
	}
}
