# Mod Info
This mod allows for Beat Saber mod developers to implement custom settings user interfaces in a simple and easy way. This mod does nothing on its own!

# Example Usage
*It's important that you setup your settings options in the SceneManager_sceneLoaded event when the "Menu" scene is loaded! It can't be the SceneManager_activeSceneChanged event!*

```cs
bool toggleValue = false;
private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) 
{
  if (arg0.name == "Menu")
  {
    Sprite icon = UIUtilities.LoadSpriteFromResources("TestAssemblyName.TestFolderName.TestImageName.png");
  
    ToggleOption toggle = GameplaySettingsUI.CreateToggleOption("Test Option", "This is a short description of the option, which will be displayed as a tooltip when you hover over it", icon);
    toggle.AddConflict("Another Gameplay Option");

    toggle.GetValue = toggleValue;
    toggle.OnToggle += ((bool e) =>
    {
      toggleValue = e;
    });

    SubMenu settingsSubmenu = SettingsUI.CreateSubMenu("Test Submenu");
    IntViewController testInt = settingsSubmenu.AddInt("Test Int", 0, 100, 1);
    testInt.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "Test Int", 0, true); };
    testInt.SetValue += delegate (int value) { ModPrefs.SetInt(Plugin.Name, "Test Int", value); };
    
    MenuButtonUI.AddButton("Test Button", delegate { Console.WriteLine("Pushed test button!"); });
  }
}
```
