using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Object representation of EPG State
/// </summary>
[XmlRoot("State"), XmlType("State")]
public class EpgState
{

    #region Private Variables

    private string name;
    private List<ActivationCriteria> activationCriterias;
    private List<DirectDestination> directDestinations;
    private Menu menu;
    private Menus menus;
    private List<Connection> interMenuNavigation;

    #endregion

    #region Properties

    [XmlAttribute("name")]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public List<ActivationCriteria> ActivationCriterias
    {
        get { return activationCriterias; }
        set { activationCriterias = value; }
    }

    public List<DirectDestination> DirectDestinations
    {
        get { return directDestinations; }
        set { directDestinations = value; }
    }

    public Menu Menu
    {
        get { return menu; }
        set { menu = value; }
    }

    public Menus Menus
    {
        get { return menus; }
        set { menus = value; }
    }

    private List<Connection> InterMenuNavigation
    {
        get { return interMenuNavigation; }
        set { interMenuNavigation = value; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the Menu Layout of this state
    /// </summary>
    /// <returns>The menu Layout as string</returns>
    public string GetMenuLayout()
    {
        if (Menu != null)
        {
            return Menu.Layout;
        }
        else
        {
            return null;
        }
    }

    public string GetMultiLineMenuLayout()
    {
        return Menus.MultiLineMenu.Layout;
    }

    public bool IsMultiLineMenu()
    {
        bool returnState = false;

        returnState = (Menus != null);

        return returnState;
    }

    /// <summary>
    /// Get the menu type of this state
    /// </summary>
    /// <returns>THe menu Type as string</returns>
    public string GetMenuType()
    {
        return Menu.Type;
    }

    public bool IsActivationCriteriaSame(EpgState state)
    {
        bool returnState = true;

        if (state == null || state.ActivationCriterias == null || this.ActivationCriterias == null)
        {
            returnState = false;
        }
        else
        {
            if (this.ActivationCriterias.Count == state.ActivationCriterias.Count)
            {
                for (int index = 0; index < this.ActivationCriterias.Count; index++)
                {
                    if (this.ActivationCriterias[index] != state.ActivationCriterias[index])
                    {
                        returnState = false;
                        break;
                    }
                }
            }
            else
            {
                returnState = false;
            }
        }

        return returnState;
    }

    public string GetDictionaryKey(string itemName)
    {
        string dictionaryKey = "";

        if (menus != null)
        {
            dictionaryKey = menus.GetDictionaryKey(itemName);
        }

        if (dictionaryKey == "")
        {
            dictionaryKey = menu.GetDictionaryKey(itemName);
        }

        return dictionaryKey;
    }

    #endregion

}

/// <summary>
/// Object representation of activation criteria of a state
/// </summary>
public class ActivationCriteria
{

    #region Private Variables

    private string key;
    private string _value;

    #endregion

    #region Properties

    [XmlAttribute("key")]
    public string Key
    {
        get { return key; }
        set { key = value; }
    }

    [XmlAttribute("value")]
    public string Value
    {
        get { return this._value; }
        set { this._value = value; }
    }

    #endregion

    #region Methods

    public static bool operator ==(ActivationCriteria ac1, ActivationCriteria ac2)
    {
        bool returnState = true;

        if (object.ReferenceEquals(ac1, null) || object.ReferenceEquals(ac2, null))
        {
            returnState = false;
        }
        else
        {
            if (ac1.Key != ac2.Key || ac1.Value != ac2.Value)
            {
                returnState = false;
            }
        }

        return returnState;
    }

    public static bool operator !=(ActivationCriteria ac1, ActivationCriteria ac2)
    {
        return !(ac1 == ac2);
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        try
        {
            return this == (ActivationCriteria)obj;
        }
        catch
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion

}

/// <summary>
/// Object representation of Direct Destinations of a state
/// </summary>
public class DirectDestination
{

    #region Private Variables

    private string name;
    private Key irKey;

    #endregion

    #region Properties

    [XmlAttribute("name")]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public Key IrKey
    {
        get { return irKey; }
        set { irKey = value; }
    }

    #endregion

}

/// <summary>
/// Object representation of Menu object in a state
/// </summary>
public class Menu
{

    #region Private Variables

    private List<Item> items;
    private string layout;
    private string type;
    private string selectionIdentifier;

    #endregion

    #region Properties

    public List<Item> Items
    {
        get { return items; }
        set { items = value; }
    }

    [XmlAttribute("layout")]
    public string Layout
    {
        get { return layout; }
        set { layout = value; }
    }

    [XmlAttribute("type")]
    public string Type
    {
        get { return type; }
        set { type = value; }
    }

    [XmlAttribute("selectionIdentifier")]
    public string SelectionIdentifier
    {
        get { return selectionIdentifier; }
        set { selectionIdentifier = value; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the Layout information of the parent item
    /// </summary>
    /// <param name="itemName">The item name as String</param>
    /// <returns>The parent item layout as String</returns>
    public string GetParentItemLayout(string itemName)
    {
        string layout = "";

        //Check if the item is a parent item
        if (IsParentItem(itemName))
        {
            //Layout is same as parent item layout
            layout = Layout;
        }
        else
        {
            //Get the layout of the parent item
            layout = GetParentItem(itemName).Menu.Layout;
        }

        return layout;
    }

    /// <summary>
    /// Get the Type of the Menu of the parent item
    /// </summary>
    /// <param name="itemName">The item name as String</param>
    /// <returns>The parent item type as String</returns>
    public string GetParentItemType(string itemName)
    {
        string type = "";

        //Check if the item is a parent item
        if (IsParentItem(itemName))
        {
            //Type is same as parent type
            type = Type;
        }
        else
        {
            //Get the type of the parent item
            type = GetParentItem(itemName).Menu.Type;
        }

        return type;
    }

    /// <summary>
    /// Get the navigation Type of a particular item
    /// </summary>
    /// <param name="itemName">The item name as string</param>
    /// <returns>The navigation type as boolean</returns>
    public bool GetNavigationType(string itemName)
    {
        bool navigationType = false;
        Item item = GetItem(itemName);

        if (item != null)
        {
            navigationType = item.NonStandardNavigation;
        }

        return navigationType;
    }

    /// <summary>
    /// Get the dictionary key for a particular item name
    /// </summary>
    /// <param name="itemName">The item name as string</param>
    /// <returns>The dictionary key of the item as string</returns>
    public string GetDictionaryKey(string itemName)
    {
        string dictionaryKey = "";
        Item item = GetItem(itemName);

        if (item != null)
        {
            dictionaryKey = item.DictionaryKey;
        }

        return dictionaryKey;
    }

    /// <summary>
    /// Get the item based on its item name
    /// </summary>
    /// <param name="itemName">The item name as string</param>
    /// <returns>The matching item object</returns>
    private Item GetItem(string itemName)
    {
        //Check if item is present in the parent
        Item matchedItem = Items.Find(item1 => item1.Name == itemName);

        //If item is not present in the parent,then iterate through the child items
        if (matchedItem == null)
        {
            foreach (Item item in Items)
            {
                if (item.Menu != null)
                {
                    //Search for the items within the child items
                    matchedItem = item.Menu.GetItem(itemName);
                }

                if (matchedItem != null)
                {
                    break;
                }
            }
        }

        return matchedItem;
    }

    /// <summary>
    /// Get the parent item of the given item name
    /// </summary>
    /// <param name="itemName">The item name as string</param>
    /// <returns>The parent item of the given item name as object. If the item itself is a parent item, then the item itself is returned.</returns>
    private Item GetParentItem(string itemName)
    {
        //Check if the item is present in the parent list
        Item matchedItem = Items.Find(item1 => item1.Name == itemName);

        //If the item found is not the parent item,then iterate through all items
        if (matchedItem == null)
        {
            foreach (Item item in Items)
            {
                if (item.Menu != null)
                {
                    //Search for the items within the child items
                    matchedItem = item.Menu.GetParentItem(itemName);
                }

                if (matchedItem != null)
                {
                    //Take the parent item as the matched item
                    matchedItem = item;
                    break;
                }
            }
        }

        return matchedItem;
    }

    /// <summary>
    /// Check if the item is a parent item or not
    /// </summary>
    /// <param name="itemName">The item name as string</param>
    /// <returns>True if the item is a parent item,false otherwise</returns>
    private bool IsParentItem(string itemName)
    {
        Item matchedItem = Items.Find(item1 => item1.Name == itemName);
        bool isParentItem = true;

        if (matchedItem == null)
        {
            isParentItem = false;
        }

        return isParentItem;
    }

    #endregion

}

/// <summary>
/// Object representation of Item Objects in a Menu of a state
/// </summary>
public class Item
{

    #region Private Variables

    private Menu menu;
    private string name;
    private string dictionaryKey;
    private bool isPopup;
    private string destination;
    private bool nonStandardNavigation;

    #endregion

    #region Properties

    public Menu Menu
    {
        get { return menu; }
        set { menu = value; }
    }

    [XmlAttribute("name")]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    [XmlAttribute("dictionaryKey")]
    public string DictionaryKey
    {
        get { return dictionaryKey; }
        set { dictionaryKey = value; }
    }

    [XmlAttribute("isPopup")]
    public bool IsPopup
    {
        get { return isPopup; }
        set { isPopup = value; }
    }

    [XmlAttribute("destination")]
    public string Destination
    {
        get { return destination; }
        set { destination = value; }
    }

    [XmlAttribute("nonStandardNavigation")]
    public bool NonStandardNavigation
    {
        get { return nonStandardNavigation; }
        set { nonStandardNavigation = value; }
    }

    #endregion

}

/// <summary>
/// Object representation of Menus object in a state
/// </summary>
public class Menus
{

    #region Private Variables

    private MultiLineMenu multiLineMenu;
    private Menu menu;

    #endregion

    #region Properties

    public MultiLineMenu MultiLineMenu
    {
        get { return multiLineMenu; }
        set { multiLineMenu = value; }
    }

    public Menu Menu
    {
        get { return menu; }
        set { menu = value; }
    }

    #endregion

    #region Methods

    public string GetDictionaryKey(string itemName)
    {
        string dictionaryKey = "";

        if (multiLineMenu != null)
        {
            dictionaryKey = multiLineMenu.GetDictionaryKey(itemName);
        }

        if (dictionaryKey == "")
        {
            dictionaryKey = menu.GetDictionaryKey(itemName);
        }

        return dictionaryKey;
    }

    #endregion

}

/// <summary>
/// Object representation of Connection object in a state
/// </summary>
public class Connection
{

    #region Private Variables

    private string source;
    private string destination;
    private string key;

    #endregion

    #region Properties

    [XmlAttribute("source")]
    public string Source
    {
        get { return source; }
        set { source = value; }
    }

    [XmlAttribute("destination")]
    public string Destination
    {
        get { return destination; }
        set { destination = value; }
    }

    [XmlAttribute("key")]
    public string Key
    {
        get { return key; }
        set { key = value; }
    }

    #endregion

}

/// <summary>
/// Object representation of a MultiLineMenu object in a Menus object
/// </summary>
public class MultiLineMenu
{

    #region Private Variables

    private string layout;
    private string type;
    private string selectionIdentifier;
    private string name;
    private List<Identifier> identifierEntries;
    private List<Line> lines;
    private List<Key> interLineNavigation;

    #endregion

    #region Properties

    [XmlAttribute("layout")]
    public string Layout
    {
        get { return layout; }
        set { layout = value; }
    }

    [XmlAttribute("type")]
    public string Type
    {
        get { return type; }
        set { type = value; }
    }

    [XmlAttribute("selectionIdentifier")]
    public string SelectionIdentifier
    {
        get { return selectionIdentifier; }
        set { selectionIdentifier = value; }
    }

    [XmlAttribute("name")]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public List<Identifier> IdentifierEntries
    {
        get { return identifierEntries; }
        set { identifierEntries = value; }
    }

    public List<Line> Lines
    {
        get { return lines; }
        set { lines = value; }
    }

    public List<Key> InterLineNavigation
    {
        get { return interLineNavigation; }
        set { interLineNavigation = value; }
    }

    #endregion

    #region Methods

    public string GetInterLineNavigation()
    {
        return InterLineNavigation[0].Name;
    }

    public string GetDictionaryKey(string itemName)
    {
        string dictionaryKey = "";

        foreach (Line line in lines)
        {
            Item item = line.Items.Find(item1 => item1.Name == itemName);

            if (item != null)
            {
                dictionaryKey = item.DictionaryKey;
                break;
            }
        }

        return dictionaryKey;
    }

    #endregion

}

public class Identifier
{

    #region Private Variables

    private string key;
    private string value;

    #endregion

    #region Properties

    [XmlAttribute("key")]
    public string Key
    {
        get { return key; }
        set { key = value; }
    }

    [XmlAttribute("value")]
    public string Value
    {
        get { return this.value; }
        set { this.value = value; }
    }

    #endregion

}

/// <summary>
/// Object representation of the Line
/// </summary>
[XmlRoot("Line")]
public class Line
{

    #region Private Variables

    private List<Item> line;

    #endregion

    #region Properties

    [XmlElement("Item")]
    public List<Item> Items
    {
        get { return line; }
        set { line = value; }
    }

    #endregion

}

/// <summary>
/// Object representation of the key
/// </summary>
public class Key
{

    #region Private Variables

    private string name;

    #endregion

    #region Properties

    [XmlAttribute("name")]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    #endregion

}
