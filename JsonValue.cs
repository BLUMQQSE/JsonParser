using System.Collections;
using System.Text;

public class JsonValue 
{

    public JsonValue this[int index]
    {
        get
        {
            if (index < 0 || index > list.Count)
            {
                return new JsonValue();
                //return new JsonValue(new OwnerInfo(this, false, index.ToString()));
            }
            else if (index >= list.Count)
            {
                for(int i = list.Count; i <= index; i++)
                    Append(new JsonValue());
                
            }

            return list[index];
        }
        set
        {
            varType = VarType.Array;
            if (index < 0 || index > list.Count)
                return;
            list[index] = value;
        }
    }
    public JsonValue this[string key]
    {
        get
        {
            //varType = VarType.Object;
            if (!content.ContainsKey(key))
            {     
                Add(key, new JsonValue());
                return content[key];
            }
            return content[key];
        }
        set
        {
            varType = VarType.Object;
            Add(key, value);
        }
    }

    public bool IsNull { get { return varType == VarType.Null; } }
    public bool IsObject { get { return varType == VarType.Object; } }
    public bool IsArray { get { return varType == VarType.Array; } }
    public bool IsValue { get { return !IsArray && !IsObject; } } 

    public List<JsonValue> Array
    {
        get { return list; }
        set 
        {
            varType = VarType.Array;
            list = value; 
        }
    }

    public Dictionary<string, JsonValue> Object
    {
        get { return content; }
        set
        {
            varType = VarType.Object;
            content = value;
        }
    }

    /// <summary> Returns total number of objects within this object and itself </summary>
    public int Size
    {
        get
        {
            if (IsNull)
                return 0;
            int val = 1;
            if(varType == VarType.Array)
            {
                for(int i = 0; i < list.Count; i++)
                    val += list[i].Size;
            }
            if(varType == VarType.Object)
            {
                
                foreach(KeyValuePair<string, JsonValue> entry in content)
                {
                    val += entry.Value.Size;
                }
            }

            return val;
        }
    }
    /// <summary> Returns number of objects directly contained in this object</summary>
    public int Count
    {
        get
        {
            if (IsNull)
                return 0;
            if (IsArray)
                return list.Count;
            if(IsObject)
                return content.Count;

            return 1;
        }
    }

    #region Enums
    enum VarType
    {
        String,
        Int,
        Decimal,
        Bool,
        Array,
        Object,
        Null
    }

    enum ContainerEnum
    {
        SettingKey,
        AddingValue
    }

    #endregion

    #region Constructors

    public JsonValue() { InitializeJson(); }

    public JsonValue(string value)
    {
        InitializeJson();
        Set(value);
    }
    public JsonValue(int value)
    {
        InitializeJson();
        Set(value);
    }
    public JsonValue(float value)
    {
        InitializeJson();
        Set(value);
    }
    public JsonValue(double value)
    {
        InitializeJson();
        Set(value);
    }
    public JsonValue(bool value)
    {
        InitializeJson();
        Set(value);
    }
    #endregion

    void InitializeJson()
    {
        valueStored = "";
        varType = VarType.Null;
        
        content = new Dictionary<string, JsonValue>();
        list = new List<JsonValue>();
    }

    VarType varType;
    string valueStored;
    Dictionary<string, JsonValue> content;
    List<JsonValue> list;

    /// <summary> Removes all data associated with this object. </summary>
    public void Clear()
    {
        InitializeJson();
    }
    
    /// <returns> Associated data for this objects as a string. </returns>
    public string AsString() 
    {
        if (valueStored.Length == 0) return "[NULL JSON STRING]";
        return valueStored; 
    }
    /// <returns> Associated data for this objects as a int. </returns>
    public int AsInt()
    {
        int result;
        try { result = int.Parse(valueStored); }
        catch (Exception ex) { result = 0; }
        return result;
    }
    /// <returns> Associated data for this objects as a uint. </returns>
    public uint AsUInt()
    {
        uint result;
        try { result = uint.Parse(valueStored); }
        catch (Exception ex) { result = 0; }
        return result;
    }
    /// <returns> Associated data for this objects as a double. </returns>
    public double AsDouble()
    {
        double result;
        try { result = double.Parse(valueStored); }
        catch (Exception ex) { result = 0; }
        
        return result;
    }
    /// <returns> Associated data for this objects as a float. </returns>
    public float AsFloat()
    {
        float result;
        try { result = float.Parse(valueStored); }
        catch (Exception ex) { result = 0; }
        return result;
    }
    /// <returns> Associated data for this objects as a bool. </returns>
    public bool AsBool()
    {
        if (valueStored.Equals("true"))
            return true;
        return false;
    }

    public void Set(JsonValue obj)
    {
        this.list = obj.list;
        this.content = obj.content;
        this.varType = obj.varType;
        this.valueStored = obj.valueStored;
    }
    public void Set(ISerializable obj) { Set(obj.Serialized()); }
    public void Set(string value) { Set<string>(value); }
    public void Set(bool value) { Set<bool>(value); }
    public void Set(int value) { Set<int>(value); }
    public void Set(uint value) { Set<uint>(value); }
    public void Set(float value) {  Set<float>(value); }
    public void Set(double value) { Set<double>(value); }
    public void Set(Decimal value) { Set<Decimal>(value); }

    void Set<T>(T value)
    {
        InitializeJson();
        valueStored = value.ToString();
        
        Type type = typeof(T);
        if (type == typeof(string))
            varType = VarType.String;
        else if (type == typeof(int) || type == typeof(uint))
            varType = VarType.Int;
        else if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
            varType = VarType.Decimal;
        else if (type == typeof(bool))
        {
            varType = VarType.Bool;
            valueStored = valueStored.ToLower();
        }
    }

    #region ADD
    public void Add(string key, JsonValue value)
    {
        if (content.ContainsKey(key))
            content[key] = value;
        else
        {
            content.TryAdd(key, value);
            varType = VarType.Object;
        }
    }
    public void Add(string key, ISerializable obj) {  Add(key, obj.Serialized()); }
    public void Add(string key, string value) { Add<string>(key, value); }
    public void Add(string key, bool value) { Add<bool>(key, value); }
    public void Add(string key, int value) { Add<int>(key, value); }
    public void Add(string key, uint value) { Add<uint>(key, value); }
    public void Add(string key, float value) { Add<float>(key, value); }
    public void Add(string key, double value) { Add<double>(key, value); }
    public void Add(string key, decimal value) { Add<decimal>(key, value); }
    void Add<T>(string key, T val)
    {
        JsonValue obj = new JsonValue();
        obj.Set(val);
        Add(key, obj);
    }

    #endregion

    #region REMOVE

    public void Remove(string key) 
    { 
        content.Remove(key); 
        if (content.Count == 0)
        {
            varType = VarType.Null;
        }
    }
    public void Remove(int index) 
    {
        if(index >= 0 && list.Count > index)
            list.RemoveAt(index);
        if (list.Count == 0)
            varType = VarType.Null;
    }

    public void Insert(int index, JsonValue obj)
    {
        if (index >= 0 && list.Count > index)
            list.Insert(index, obj);
    }
    public void Insert(int index, ISerializable obj) {  Insert(index, obj.Serialized()); }
    public void Insert(int index, string value) { Insert<string>(index, value); }
    public void Insert(int index, bool value) { Insert<bool>(index, value); }
    public void Insert(int index, int value) { Insert<int>(index, value); }
    public void Insert(int index, uint value) { Insert<uint>(index, value); }
    public void Insert(int index, float value) { Insert<float>(index, value); }
    public void Insert(int index, double value) { Insert<double>(index, value); }
    public void Insert(int index, decimal value) { Insert<decimal>(index, value); }
    void Insert<T>(int index, T val)
    {
        JsonValue obj = new JsonValue();
        obj.Set(val);
        Insert(index, obj);
    }


    #endregion

    #region APPEND

    public void Append(JsonValue value)
    {
        list.Add(value);
        varType = VarType.Array;
    }
    public void Append(ISerializable value) { Append(value.Serialized()); }
    public void Append(string value) { Append<string>(value); }
    public void Append(bool value) { Append<bool>(value); }
    public void Append(int value) { Append<int>(value); }
    public void Append(uint value) { Append<uint>(value); }
    public void Append(float value) { Append<float>(value); }
    public void Append(double value) { Append<double>(value); }
    public void Append(decimal value) { Append<decimal>(value); }
    void Append<T>(T value)
    {
        JsonValue obj = new JsonValue();
        obj.Set(value);
        Append(obj);
    }

    #endregion


    #region SERIALIZATION
    /// <summary> Converts this JsonValue object into a json string. 
    /// This method should only be called on an Object value, an Array and Value will return "{}".</summary>
    public string Serialize()
    {
        if (varType != VarType.Object)
            return "{}";
        string data = Serializer();
        return data;
    }
    /// <summary> Helper function for handling creating a json string of all connected JsonValue objects.</summary>
    string Serializer()
    {
        StringBuilder sb = new StringBuilder();
        switch (varType)
        {
            case VarType.Null:
            case VarType.String:
            case VarType.Bool:
            case VarType.Int:
            case VarType.Decimal:
                if (varType == VarType.Null)
                    return "null";
                if (varType == VarType.String)
                {
                    // convert escaped characters to text
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append('\"');
                    for(int i = 0; i < valueStored.Length; i++)
                    {
                        switch (valueStored[i])
                        {
                            case '\n':
                                stringBuilder.Append("\\n");
                                continue;
                                break;
                            case '\t':
                                stringBuilder.Append("\\t");
                                continue;
                                break;
                            case '\\':
                                stringBuilder.Append("\\");
                                stringBuilder.Append("\\");
                                continue;
                                break;
                            default:
                                stringBuilder.Append(valueStored[i]);
                                break;
                        }
                    }
                    stringBuilder.Append('\"');
                    return stringBuilder.ToString();
                }
                if (varType == VarType.Bool)
                {
                    if (valueStored.Equals("False") || valueStored.Equals("false"))
                        return "false";
                    else
                        return "true";
                }
                return valueStored;
            case VarType.Array:
                {
                    sb.Append('[');
                    foreach (JsonValue item in list)
                    {
                        sb.Append(item.Serializer());
                        sb.Append(',');
                    }
                    sb.Length--;
                    sb.Append(']');

                    return sb.ToString();
                }
            case VarType.Object:
                {
                    sb.Append('{');
                    List<string> keys = new List<string>(content.Keys);
                    foreach (string key in keys)
                    {
                        sb.Append('\"'+key+"\":" + content[key].Serializer() + ',');
                    }
                    if (keys.Count > 0)
                    {
                        sb.Length--;
                    }
                    sb.Append('}');

                    return sb.ToString();
                }
        }
        return "null";
    }

    #endregion

    #region DESERIALIZATION
    /// <summary>
    /// This method taked in a string and attempts to create a JsonValue obj to contain the data.
    /// </summary>
    /// <param name="data">Json string. Formatting will be removed within this function.</param>
    /// <returns>True if successful, False if unsuccessful.</returns>
    public bool Deserialize(string data)
    {
        varType = VarType.Object;
        int index = 1;
        string unformattedData = RemoveFormatting(data);
        try
        {
            Deserializer(unformattedData, ref index, VarType.Object);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    /// <summary>
    /// Helper function for converting a string into a JsonValue object(s).
    /// </summary>
    void Deserializer(string data, ref int index, VarType type)
    {
        bool inString = false;
        switch (type)
        {
            case VarType.Object:
                {
                    ContainerEnum state = ContainerEnum.SettingKey;

                    string key = "";
                    while (data[index] != '}' || inString)
                    {
                        if(state == ContainerEnum.SettingKey)
                        {
                            key = "";
                            int startIndex = index;
                            while (data[index] != ':' || inString)
                            {
                                UpdateInString(ref inString, data, index);
                                index++;
                            }
                            // set key, and remove qoutations surrounding it
                            key = data.Substring(startIndex + 1, index - startIndex - 2);
                            state = ContainerEnum.AddingValue;
                        }
                        else
                        {
                            //remove colon
                            index++;

                            JsonValue valToAdd = new JsonValue();
                            if (data[index] == '{')
                            {
                                index++;
                                valToAdd.Deserializer(data, ref index, VarType.Object);
                            }
                            else if (data[index] == '[')
                            {
                                index++;
                                valToAdd.Deserializer(data, ref index, VarType.Array);
                            }
                            else
                                valToAdd.Deserializer(data, ref index, VarType.Null);

                            Add(key, valToAdd);

                            state = ContainerEnum.SettingKey;
                        }
                        
                        if (data[index] == ',')
                            index++;
                    }
                    index++;
                }
                break;
            case VarType.Array:
                {
                    int startIndex = index;
                    while(data[index] != ']')
                    {
                        while (data[index] != ',' && data[index] != ']')
                        {

                            JsonValue valToAdd = new JsonValue();
                            if (data[index] == '{')
                            {
                                // need to move to creating
                                index++;
                                valToAdd.Deserializer(data, ref index, VarType.Object);
                            }
                            else if (data[index] == '[')
                            {
                                index++;
                                valToAdd.Deserializer(data, ref index, VarType.Array);
                            }
                            else
                                valToAdd.Deserializer(data, ref index, VarType.Null);

                            Append(valToAdd);
                            
                        }
                        if (data[index] == ',')
                            index++;
                    }

                    index++;
                }
                break;
            default:
                {
                    StringBuilder value = new StringBuilder();
                    while (data[index] != ',' && data[index] != '}' && data[index] != ']' || inString)
                    {
                        UpdateInString(ref inString, data, index);
                        if (inString)
                        {
                            if (data[index] == '\\')
                            {
                                switch (data[index + 1]) 
                                {
                                    case 'n':
                                        index+=2;
                                        value.Append('\n');
                                        continue;
                                        break;
                                    case 't':
                                        index+=2;
                                        value.Append('\t');
                                        continue;
                                        break;
                                    case '\\':
                                        index+=2;
                                        value.Append('\\');
                                        continue;
                                        break;
                                }

                            }
                        }
                        value.Append(data[index]);
                        index++;
                    }

                    valueStored = value.ToString();
                    if (valueStored[0] == '\"')
                    {
                        // remove quotations from string
                        valueStored = valueStored.Substring(1, valueStored.Length - 2);
                        varType = VarType.String;
                    }
                    else if (valueStored == "null" || valueStored.Length == 0)
                        varType = VarType.Null;
                    else if (valueStored.Contains('.'))
                        varType = VarType.Decimal;
                    else if (valueStored.Equals("true") || valueStored.Equals("false"))
                        varType = VarType.Bool;
                    else
                        varType = VarType.Int;

                }
                break;
        }
    }

    #endregion
    /// <summary>
    /// Private function for handling determining when within a string value while
    /// deserializing from a json string.
    /// </summary>
    void UpdateInString(ref bool inString, string data, int index)
    {
        if(data[index] == '\"')
        {
            if (index > 0)
            {
                if (data[index - 1] != '\\')
                    inString = !inString;
            }
            else
                inString = !inString;
        }
    }

    #region FORMATTING
    /// <summary>
    /// Takes in a string and removes all special characters and spaces which are not
    /// within a value's string and returns the new unformatted string.
    /// </summary>
    static string RemoveFormatting(string data)
    {
        bool inString = false;
        var result = new StringBuilder(data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == '\"')
            {
                if (data[i - 1] != '\\')
                    inString = !inString;
            }
            if (!inString)
            {
                if (data[i] != ' ' && data[i] != '\t' && data[i] != '\n' && data[i] != '\r')
                    result.Append(data[i]);
            }
            else
                result.Append(data[i]);

        }

        return result.ToString();

    }
    /// <summary>
    /// Takes in a string and adds on appropriate formatting to make a string more legible
    /// in a json file.
    /// </summary>
    static public string AddFormatting(string data)
    {
        StringBuilder result = new StringBuilder(data.Length);
        const string TAB = "    ";
        int tabDepth = 1;
        result.Append(data[0].ToString() + '\n' + TAB);
        int i = 1;
        while(i < data.Length)
        {
            switch (data[i]) 
            {
                case '{':
                case '[':
                    if (data[i - 1] != '[' && data[i - 1] != '{' && data[i - 1] != ',')
                    {
                        result.Append('\n');
                        for (int j = 0; j < tabDepth; j++)
                            result.Append(TAB);
                    }
                    result.Append(data[i].ToString() + '\n');
                    tabDepth++;
                    for (int j = 0; j < tabDepth; j++)
                        result.Append(TAB);
                    break;
                case ':':
                    result.Append(": ");
                    break;
                case ',':
                    result.Append(",\n");
                    for (int j = 0; j < tabDepth; j++)
                        result.Append(TAB);
                    break;
                case '}':
                case ']':
                    tabDepth--;
                    result.Append('\n');
                    for (int j = 0; j < tabDepth; j++)
                        result.Append(TAB);
                    result.Append(data[i].ToString());
                    break;
                default:
                    result.Append(data[i].ToString());
                    break;
            }

            i++;
        }
        return result.ToString();
    }

    #endregion

}

public interface ISerializable
{
    /// <returns> A JsonValue representation of data stored by this object. </returns>
    JsonValue Serialized();
}