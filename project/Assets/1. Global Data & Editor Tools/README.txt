 1. Global Data & Editor Tools
 
 Global Data:
    • A list of 10 adjectives
    • A list of 10 nouns
    • A list of 10 colors: alpha must always be 100%, no black/white/grays, or reds hues are 
allowed

• Item MonoBehaviour:
    • Public data:
        ▪ Name
        ▪ Color
    • Generate()
        ▪ Choose a random color from the global list
        ▪ Generates a name from the adjectives and nouns using proper case
        ▪ Renames the GameObject using its Name in lower camel case
        ▪ Unique names only

• Create 20 or so GameObjects with this MonoBehaviour 
• Create an editor tool
    • Lists all these MonoBehaviours in a scene
    • Displays each item’s name in the given color
    • Filter list on search query with partial name match
    • Filter list based on color options
    • On item click, select the associated GameObject

Document decisions and complex functionality.

Provide all queries and links you used to use as talking points.
The use of AI is not prohibited, but it is frowned upon for these challenges. Provide queries if
used.

//=========///DECISIONS///==========

The initial request requires global variables. In this case, scriptable objects will be used.
The problem with the scriptable objects is that it can be changed at runtime by mistake, so I will be sealing the properties with a lambda (name?)



*=========///NOTES///==========
 
 Condense the item types into a single item with traits.
 A single scriptable object
 
 Adjust the search objects.