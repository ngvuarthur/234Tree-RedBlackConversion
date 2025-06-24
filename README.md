# 2-3-4 Tree Implementation in C# with Red-Black Tree Conversion

## 📚 Overview

This project implements a **2-3-4 Tree** (also known as a 4-way tree) in C#, including:

- Insertion
- Deletion
- Search
- Pretty-printing of the tree

Additionally, it includes a method to **convert the 2-3-4 Tree into a Red-Black Tree**, reflecting its logical equivalence and showcasing binary search tree behavior.

---

## 🚀 Features

### 🌳 TwoThreeFourTree
- ✅ **Insert**: Add a key into the tree and rebalance if needed.
- 🧹 **Delete**: Remove a key while maintaining tree integrity.
- 🔍 **Search**: Find whether a given key exists.
- 🖨️ **Print**: Visualize the tree structure in a rotated, indented format.
- 🔄 **Convert**: Transform the 2-3-4 tree into an equivalent Red-Black Tree.

### 🌈 Red-Black Tree (BSTforRBTree)
- Binary Search Tree implementation tagged with `RED` or `BLACK` node colors.
- In-order traversal printing with color indicators.

---

## 🧪 Usage Example

```csharp
var tree = new TwoThreeFourTree<int>();

// Insert values
int[] keysToInsert = { 50, 40, 60, 30, 70, 20, 80 };
foreach (var key in keysToInsert)
{
    tree.Insert(key);
}

// Print tree
Console.WriteLine("Tree after insertions:");
tree.Print();

// Search
Console.WriteLine(tree.Search(60));  // true
Console.WriteLine(tree.Search(15));  // false

// Delete
tree.Delete(20);
tree.Delete(60);
Console.WriteLine("Tree after deletions:");
tree.Print();

// Convert to Red-Black Tree
var rbTree = tree.Convert();
Console.WriteLine("Red-Black Tree:");
rbTree.Print();

## 🧠 How It Works

### 📌 2-3-4 Tree

- A **balanced search tree** where each node can have **2, 3, or 4 children**.
- Each node holds **1 to 3 sorted keys**.
- During insertion, **split operations** are used to maintain balance.
- All leaves are at the same depth, ensuring efficient operations.

### 📌 Red-Black Tree Conversion

Each 2-3-4 node is transformed into one or more nodes in an equivalent **Red-Black Tree**:

- **2-node** → 1 **Black** node
- **3-node** → 1 **Black** node + 1 **Red** child
- **4-node** → 1 **Black** node + 2 **Red** children

The conversion:
- Is **recursive**
- Maintains **binary search tree properties**
- Is **color-aware**, reflecting the logical mapping between multiway trees and binary trees
