

# Crank.OperationResult

**crank**

*verb (used without object)*
to turn a crank, as in starting an automobile engine.

*noun*
Informal. an ill-tempered, grouchy person.

---

**What**

**Crank.OperationResult** is a C# implementation (of sorts) of a discrimination union allowing different response values wrapped in one response class.

**Why**
When straying off the happy code path, it can sometimes be difficult to differentiated between various  error conditions. You may not want use exceptions, so find yourself having to cater for numerous conditions perhaps using tuples or some struct or class type. OperationResult is a wrapper for this approach.


## **OperationResult (untyped) **
**State**: An OperationResult contains a state field indicating *undefined*, *success* or *failure*. 
When an result is first created, either with **new**, or using the **Undefined()** static method, the OperationResult is initialised in an *undefined* state.
**Value  **:  A field referencing a *GenericValue* class, that allows different values be stored and extracted based on the value type.

## OperationResult < TExpectedValue > 
**State** As above
**Value** A successful result is constrained to being of type TExpectedValue. A failure value can be of any type. 



### Initialising an OperationResult
An OperationResult can be created a number of ways.

    // Undefined operations
    var result = new OperationResult();
    var result = new OperationResult<string>();
    var result = OperationResult.Undefined();
    var result = OperationResult.Undefined<string>("a string value"); //typed result
    
    // Successful operations
    var result = OperationResult.Succeeded();
    var result = OperationResult.Succeeded<string>("a string value"); // typed result
    
    // Failed Operations
    var result = OperationResult.Failed();
    var result = OperationResult.Failed<string>();

### Changing an OperationResults state
An OperationResult state can be changed using the Success, Fail and Set methods.

### Change state to successful
    
	// success no value
    var untypedResult = OperationResult.Undefined();
    unTypedResult.Success(); 
    
    // success with an int value - no constraint
    var untypedResult = OperationResult.Undefined();
    unTypedResult.Success<int>(anIntValue);  
    
    // success with a guid value - constraint
    var typedResult = OperationResult.Undefined<Guid>();
    typedResult.Success(aGuidValue); 

### Change state to failure
    
	// failure with no value
    var untypedResult = OperationResult.Undefined();
    unTypedResult.Fail();   
    
    // failure with an int value
    var untypedResult = OperationResult.Undefined();
    unTypedResult.Fail<int>(anIntValue); 
    
    // failure with no value - no constraint
    var untypedResult = OperationResult.Undefined<string>();
    typedResult.Fail(); 
    
    // failure with an int value - no constraint 
    var typedResult = OperationResult.Undefined<string>();
    typedResult.Fail<int>(anIntValue); 
    
### Checking State
The state of an OperationResult can be changed via the State property, or by the IsUndefined, HasSuccessed or HasFailed methods.

### The Value property
The value property is implemented differently for both typed and untyped OperationResults.
- For untyped results, the Value property is a GenericValue< TTypeValue >. 
- For a typed result, the Value is the actual type contained within the internal GenericValue if set.

The actual value can be retrieved using either of the TryGetValue methods.

    var untypedResult = OperationResult.Undefined();
    untypedResult.Success(anIntValue);    
    untypedResult.TryGetValue<int>(out var storedIntValue);
    untypedResult.Value.TryGetValue(out var storedIntValue);
    _ = typedResult.Value.IsUndefined;
    
	var typedResult = OperationResult.Undefined<string>();
	typedResult.Success(aStringValue);
	_ = typedResult.Value;
	_ = typedResult.ValueIsUndefined;
	typedResult.TryGetValue<string>(out var stringValue);
	

### Change state using the Set method

	// setting success or failure based on aBooleanCondition
	var untypedResult = OperationResult.Undefined();
	untypedResult.Set(aBooleanCondition); 
	
	// setting success or failure based on aBooleanCondition with value
    var untypedResult = OperationResult.Undefined();
    untypedResult.Set<string>(aBooleanCondition, "a string value");
    untypedResult.Set<int>(aBooleanCondition, anIntValue);

	// setting success of failure, a constraint may or may not apply
    var typedResult = OperationResult.Undefined<string>();
    typedResult.Set<string>(aBooleanCondition, "a string value");
    typedResult.Set<int>(aBooleanCondition, 1234);

### OperationResultOptions
A typed OperationResult can only set a Success value based on the generic type parameters specified when creating the class. However using the Set method it could be possible to circumvent this behaviour. As a result an option flag called ExpectedResultTypeChecking exists that can control this behaviour;

    var typedResult = OperationResult.Undefined<string>();
    typedResult.SetOptions(opt => opt.ExpectedResultTypeChecking = OperationResultTypeChecking.Strict);
    typedResult.Set(true, aGuidValue);

The ExpectedResultTypeChecking flag allows for one of three different options
|Flag Setting |Behaviour if there is a type mismatch |
|--|--|
| Strict (default) | throws a **OperationResultExpectedTypeMismatchException** exception |
| Discard | Sets the state but *discards* the value parameter |
| Ignore | Sets the state and the value. However the Value parameter is trying to return a TExpectedValue, so if the types are mismatch, Value will return default. To retrieve the actual value by type call TryGetValue< NewType >(...) |


## Mapping from other OperationResults
To map from one OperationResult to another, use the Map, MapTo, MapConvert or MapAsync methods.

	// sets state from secondResult
    var untypedResult = OperationResult.Undefined<string>();
    var secondResult = OperationResult.Successful();
    untypedResult.Map(secondResult);
    var succeeded = unTypedResult.HasSucceeded; //true
    
    // sets state and value from secondResult
    var untypedResult = OperationResult.Undefined<string>();
    var secondResult = OperationResult.Fail<int>(404);
    untypedResult.Map(secondResult);
    var failed = untypedResult.HasFailed();
	var resultValue = untypedResult.TryGetValue(out var intValue);

### Mapping functions 

**OperationResult (Untyped)**

    OperationResult Map(OperationResult operationResult)

-- to do

    OperationResult Map<TMapType>(OperationResult<TMapType> operationResult)

-- to do

    OperationResult<TMapType> MapTo<TMapType>(OperationResult<TMapType> operationResult)

-- to do

    Task<OperationResult> MapAsync(Func<Task<OperationResult>> mapAction)

-- to do


**OperationResult< TExpectedType > (Typed)**

     new OperationResult<TExpectedValue> Map<TMapType>(OperationResult<TMapType> operationResult)

-- to do

    OperationResult<TExpectedValue> MapConvert<TNewSuccessValue>(
                OperationResult<TNewSuccessValue> operationResult,
                Func<TNewSuccessValue, TExpectedValue> convertAction)
-- to do

## Matching Operations
-- to do