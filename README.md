


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

An example of mapping 

### Mapping functions 

#### When mapping the following *default* rules apply:
1. The target result (this) must be Undefined or Successful. The mapping will not invoke if target is in a state of failure.
2. The (source) mapFromResult must not be null or undefined.

#### When mapping 
1. Copies state from source to target
2. *May* copy value from source to target

#### OperationResult Mappings for untyped results

##### Map:
Copies state and value. Returns an instance of source:

    OperationResult Map(OperationResult mapFromResult)
    OperationResult Map<TMapType>(OperationResult<TMapType> mapFromResult)

##### MapTo:
Copies state and value. Returns a **new** *typed* result instance:

    OperationResult<TMapType> MapTo<TMapType>(OperationResult<TMapType> mapFromResult)

##### MapAsync:
Copies state and value, mapAction will not invoke or mapping may not complete subject to default rules.

    Task<OperationResult> MapAsync(Func<Task<OperationResult>> mapFromResult)



#### Additional OperationResult Mappings for typed results

##### Map:
As the operation result is constrained to a successful result type, this effects the login of the typed mapping operation. 
|mapFrom state| types match? | operation |
|--|--|--|
|Undefined| n/a | none |
|Success | yes | copy state and value |
|Success | **no** | copy state, **ignore value** |
|Failure | yes | copy state and value |
|Failure | **no** | copy state and value |

If *TExpectedValue* and *TMapType* match, value will be copied. 
If TExpectedeValue and TMapType do not match, value will only be copied if the mapFromResult is failure. 

     new OperationResult<TExpectedValue> Map<TMapType>(OperationResult<TMapType> mapFromResult)
     
##### MapConvert:
This allows a mapping to convert the mapFromResult if the operation is successful
     
|mapFrom state| types match? | operation |
|--|--|--|
|Undefined| n/a | none |
|Success | yes | copy state and invoke convertAction |
|Success | **no** | copy state and invoke convertAction |
|Failure | yes | copy state and value |
|Failure | **no** | copy state and value |

    OperationResult<TExpectedValue> MapConvert<TNewSuccessValue>(
                OperationResult<TNewSuccessValue> operationResult,
                Func<TNewSuccessValue, TExpectedValue> convertAction)


## Matching Operations
Calling the Match or MatchTo methods allow fluent queries against the state and stored value types of an OperationResult.

The match operation returns a boolean value if a match has occurred within the matchAction:  

      bool Match(Action<OperationResultMatch> matchAction)
      bool Match(Action<OperationResultMatch<TExpectedValue>> matchAction)

  
 The matchTo operation returns a typed value that can be set within the matchAction:      
 

    TMatchResult MatchTo<TMatchResult>(Action<OperationResultMatchTo<TMatchResult>> matchAction, TMatchResult defaultResult = default)
    TMatchResult MatchTo<TMatchResult>(Action<OperationResultMatchTo<TExpectedValue, TMatchResult>> matchAction, TMatchResult defaultResult = default)

 
Inside the respective Match and MatchTo methods, it is possible to query the OperationResult state and contained values using the TypeIs, StateIs and TypeAndStateAre methods.

    OperationResultMatch TypeIs<TMatchType>(Action<TMatchType> matchAction
	OperationResultMatch StateIs(OperationState operationState, Action<OperationResult> matchAction)
	OperationResultMatch TypeAndStateAre<TMatchType>(OperationState operationState, Action<TMatchType> matchAction)

#### Default :
In additional to the various Type and State comparison methods, a Default method invokes a defaultAction delegate if the previous TypeIs, StateIs and TypeAndStateAre methods are not invoked.

	OperationResultMatch Default(Action<OperationResult> defaultAction = default)

#### Example:
	var result = OperationResult.Undefined<UserModel>();
	var response = result
		.Map(await getUserById(userId)
		.MatchTo<IActionResult>(m => m.
	        .Match<UserModel>(
	            value => m.Result = OK(value))
	        .Match<int>(
		        value => m.Result = new StatusCodeResult(value))
	        .Default(
	            res => response = new StatusCodeResult(500))
        );
    return response;
