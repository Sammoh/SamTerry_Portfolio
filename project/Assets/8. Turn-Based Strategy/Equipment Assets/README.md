# Equipment Assets

This directory contains ScriptableObject equipment assets for the Turn-Based Strategy system.

## Structure

- **Weapons/**: Weapon equipment assets (EquipmentSlot.Weapon)
- **Armor/**: Armor equipment assets (EquipmentSlot.Armor)  
- **Accessories/**: Accessory equipment assets (EquipmentSlot.Accessory)

## Creating Equipment Assets

### Method 1: Through Unity Editor
1. Right-click in the Project window
2. Go to **Create > Turn-Based Strategy > Equipment**
3. Configure the equipment properties in the Inspector
4. Save the asset

### Method 2: Through Equipment Editor Tool
1. Go to **Tools > Turn-Based Strategy > Equipment Editor**
2. Select an Equipment Database
3. Fill out the equipment creation form
4. Click "Create Equipment" and choose save location

### Method 3: Generate Default Assets
1. Go to **Tools > Turn-Based Strategy > Generate Default Equipment Assets**
2. This will create sample equipment assets in the appropriate folders

## Equipment Properties

Each equipment asset contains:
- **Equipment Name**: Display name of the equipment
- **Slot**: Which equipment slot it occupies (Weapon, Armor, Accessory)
- **Stat Modifiers**: Array of stat modifications the equipment provides
- **Description**: Text description of the equipment

## Using Equipment Assets

Equipment assets can be:
- Referenced by EquipmentDatabase ScriptableObjects
- Assigned directly to characters in the Inspector
- Loaded at runtime using Resources.Load or Addressables
- Used in equipment creation tools and editors

## Migration from Old System

The equipment system has been converted from regular classes to ScriptableObjects:
- **Before**: Equipment instances were created in code using constructors
- **After**: Equipment instances are ScriptableObject assets that can be created and managed in the Unity Editor

This change provides:
- Better organization and management of equipment items
- Version control for individual equipment pieces
- Easy asset referencing across the project
- Unity Editor integration for equipment creation