<Query Kind="Statements" />

/// <summary>
/// Generates a list of checkboxes for use with Live deployments when syncronizing Instance Accessor Tables
/// so that you can keep track of which tables will need to be updated on Live 
/// </summary>
EntityTypes.Where(t => t.GenerateInstanceAccessors || t.IsEnumReference).Select(t => t.TableName).Do(x => PanelManager.StackWpfElement(new System.Windows.Controls.CheckBox() { Content = x }, "Instance Accessors"));

