![](/.github/banner.png?raw=true "")
![](/.github/gallery.png?raw=true "")


# ℹ️ Features

* ScriptableObject persistence for data.
* Data abstraction in code.
* Configure data sources in Inspector: static, asset, or function (reflection).
* 🤞 Reasonably lightweight.



<br/>

# 📦 Install

1. Open Package Manager
2. Paste git URL (`<github_url>#<desired_tag>`)


<br/>

# 🚀 Usage

🧩 **Note**: To use plugin in code you need to add an assembly reference.

### Reading data

```cs
using Smidgenomics.Unity.Data;

public class VariableTest : MonoBehaviour
{
    public Readable<int> num1;
    public Readable<int> num2;

    private void Awake()
    {
        Debug.Log("Number 1: " + num1.Value);
        Debug.Log("Number 2: " + num2); // implicit conversion
    }	
}

```

### Custom types

```cs
using UnityEngine;
using System;
using Smidgenomics.Unity.Data;

[Serializable]
class MySerializedType
{
    public int a, b, c;
}


// inherit from ScriptableValue<>
class MyCustomAsset : ScriptableValue<MySerializedType> { }
```

