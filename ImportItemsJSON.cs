#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ImportItemsJson
{
    private const string JsonFileName = "items.json";
    private const string OutputFolder = "Assets/Data/ImportedItems";
    private const string DatabasePath = OutputFolder + "/ItemDatabase.asset";

    [MenuItem("WizardTower/Import/Items JSON To ScriptableObjects")]
    public static void Import()
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, JsonFileName);

        if (!File.Exists(jsonPath))
        {
            Debug.LogError("Import failed. items.json not found at: " + jsonPath);
            return;
        }

        EnsureFolder("Assets/Data");
        EnsureFolder(OutputFolder);

        string json = File.ReadAllText(jsonPath);
        ItemsRoot root = JsonUtility.FromJson<ItemsRoot>(json);

        if (root == null || root.items == null)
        {
            Debug.LogError("Import failed. Could not parse JSON into ItemsRoot or root.items is null.");
            return;
        }

        if (root.items.ingredients == null)
        {
            Debug.LogError("Import failed. root.items.ingredients is null. Check JSON field names.");
            return;
        }

        if (root.items.potions == null)
        {
            Debug.LogError("Import failed. root.items.potions is null. Check JSON field names.");
            return;
        }

        ItemDatabaseSO db = AssetDatabase.LoadAssetAtPath<ItemDatabaseSO>(DatabasePath);
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<ItemDatabaseSO>();
            AssetDatabase.CreateAsset(db, DatabasePath);
        }

        db.ingredients.Clear();
        db.potions.Clear();

        Dictionary<string, IngredientSO> ingredientByName = new Dictionary<string, IngredientSO>();

        // Create or update IngredientSO assets
        foreach (Ingredient ing in root.items.ingredients)
        {
            if (ing == null || string.IsNullOrWhiteSpace(ing.name)) continue;

            string id = ToId(ing.name);
            string assetPath = OutputFolder + "/Ingredient_" + id + ".asset";

            IngredientSO so = AssetDatabase.LoadAssetAtPath<IngredientSO>(assetPath);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<IngredientSO>();
                AssetDatabase.CreateAsset(so, assetPath);
            }

            so.displayName = ing.name;
            so.itemType = ing.itemType;
            so.itemPrice = ing.itemPrice;
            so.maxPerCharacter = ing.maxPerCharacter;
            so.rarity = ing.rarity;

            EditorUtility.SetDirty(so);

            ingredientByName[ing.name.Trim()] = so;
            db.ingredients.Add(so);
        }

        // Create or update PotionSO assets and resolve recipe references
        foreach (Potion p in root.items.potions)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.name)) continue;

            string id = ToId(p.name);
            string assetPath = OutputFolder + "/Potion_" + id + ".asset";

            PotionSO pso = AssetDatabase.LoadAssetAtPath<PotionSO>(assetPath);
            if (pso == null)
            {
                pso = ScriptableObject.CreateInstance<PotionSO>();
                AssetDatabase.CreateAsset(pso, assetPath);
            }

            pso.displayName = p.name;
            pso.rarity = p.rarity;
            pso.sellPrice = p.sellPrice;

            List<PotionRecipeEntrySO> recipeList = new List<PotionRecipeEntrySO>();

            if (p.recipe != null)
            {
                foreach (RecipeEntry entry in p.recipe)
                {
                    if (entry == null || string.IsNullOrWhiteSpace(entry.ingredientName)) continue;

                    string key = entry.ingredientName.Trim();

                    if (!ingredientByName.TryGetValue(key, out IngredientSO ingSo))
                    {
                        Debug.LogError("Potion '" + p.name + "' references missing ingredient: " + key);
                        continue;
                    }

                    recipeList.Add(new PotionRecipeEntrySO
                    {
                        ingredient = ingSo,
                        quantity = entry.quantity
                    });
                }
            }

            pso.recipe = recipeList.ToArray();

            EditorUtility.SetDirty(pso);
            db.potions.Add(pso);
        }

        EditorUtility.SetDirty(db);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Import complete. Ingredients: " + db.ingredients.Count + "  Potions: " + db.potions.Count);
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        string parent = Path.GetDirectoryName(folderPath).Replace("\\", "/");
        string name = Path.GetFileName(folderPath);

        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, name);
    }

    private static string ToId(string name)
    {
        return name.Trim().ToLower().Replace(" ", "_");
    }
}
#endif