import math
import json
import sys
import os
import re
from fractions import Fraction
from sympy import Symbol, integrate, sympify, diff

### Globals ###

# Math Functions
trigFunctions = ("sin", "cos", "tan")
inverseTrigFunctions = ("asin", "acos", "atan")
logFunctions = ("ln", "log")
miscFunctions = ("sqrt", "root", "√")
allFunctions = trigFunctions + inverseTrigFunctions + logFunctions + miscFunctions

pi = ("π", "pi")
eulerFilter = r"\be\b"

# Bracketry
closingBrackets = {")", "}", "]"}
openingBrackets = {"(", "{", "["}

# Files
settingsFile = "settings.json"

### Logic ###

# Default settings
settings = {
    "Decimals": 4,                  # integer
    "AngleUnit": "degrees",         # degrees, radians
    "ScientificNotation": False,    # bool
    "ExactFraction": False,         # bool
    "AutoDecimals": True            # bool
}

# Read json settings
# Prerequisite: JSON file or None
# Return type: Write file or None
# Note: JSON is syntax sensitive
def LoadSettings(settingsFile):
    global settings
    try:
        if not os.path.exists(settingsFile):
            with open(settingsFile, 'w') as f:
                json.dump(settings, f, indent=4)
        else:
            with open(settingsFile, 'r') as f:
                settings = json.load(f)
    except Exception as e:
        print(f"Error loading settings: {str(e)}")

# Calculator Controller
# Prerequisites: String input
# Returns: Evaluated expression or error
# Handles: Mathmatically invalid or unrecognized expression
def Calculator(expression):
    try:
        expression = ConvertSymbols(expression)
        if CheckNumber(expression):
            return expression

        if "int" in expression:
            return Integral(expression)

        if "d/dx" in expression:
            return Derivative(expression)

        tokens = SplitParetheses(expression)
        result = EvaluateExpression(tokens)
        result = FormatResult(result)
        return result
    except Exception as e:
        return f"Error: invalid expression"

# Sybolic conversion
# Prerequisites: String Input
# Returns: A numerical conversion of special symbols for pi, e
# Handles: None
def ConvertSymbols(expression):
    for symbol in pi:
        expression = expression.replace(symbol, str(math.pi))
    expression = re.sub(eulerFilter, str(math.e), expression)
    return expression

# Parentheses handler
# Prerequisites: String Input
# Returns: A tokenized list of sub-expressions to be evaluated individually
# Handles: Unclosed parentheses are implicitly closed at end of equation, throws error if closed > open
def SplitParetheses(expression):
    tokens = []
    num = ""
    func = ""
    i = 0
    while i < len(expression):
        char = expression[i]
        if char in "0123456789.":
            num += char
        elif char == '-':
            if i == 0 or expression[i-1] in "+-*/^(":
                num += char
            else:
                if num:
                    tokens.append(num)
                    num = ""
                tokens.append(char)
        else:
            if num:
                tokens.append(num)
                num = ""
            if char in "+-*/^(){}[]√":
                tokens.append(char)
            elif char.isalpha():
                func += char
                if func in allFunctions:
                    tokens.append(func)
                    func = ""
            else:
                func = ""
        i += 1
    if num:
        tokens.append(num)
    return tokens

# Simple dumb check to see if a character is numeric because python doesn't recognize decimals as numeric
def CheckNumber(n):
    try:
        float(n)
        return True
    except ValueError:
        return False

# Performs math operations
# Prerequisites: One or more list items as expressions
# Returns: Evaluated expression as string without user-specified formatting
def EvaluateExpression(tokens):
    # Formats to postfix so it can be evaluated on a stack
    def ConvertPostfix(tokens):
        precedence = {'+': 1, '-': 1, '*': 2, '/': 2, '^': 3}
        output = []
        operators = []
        prev_token = None
        for token in tokens:
            if CheckNumber(token):
                if prev_token and (prev_token in closingBrackets or prev_token in allFunctions):
                    operators.append('*')
                output.append(token)
            elif token in precedence:
                while (operators and operators[-1] in precedence and precedence[token] <= precedence[operators[-1]]):
                    output.append(operators.pop())
                operators.append(token)
            elif token == '(':
                if prev_token and (prev_token.isnumeric() or prev_token in closingBrackets):
                    operators.append('*')
                operators.append(token)
            elif token == ')':
                while operators and operators[-1] not in openingBrackets:
                    output.append(operators.pop())
                operators.pop()
            elif token in allFunctions:
                if prev_token and (prev_token.isnumeric() or prev_token in closingBrackets):
                    operators.append('*')
                operators.append(token)
            prev_token = token
        while operators:
            output.append(operators.pop())
        return output

    # Actually evaluates the now-formatted equation
    # Lots of nested logic. Could probably be cleaned up. But it's also very modular - easy to add additional functionality
    def EvaluatePostfix(postfix):
        stack = []
        for token in postfix:
            if CheckNumber(token):
                stack.append(float(token)) # required type cast
            else:
                # Standard operators
                if token in '+-*/^':
                    b = stack.pop()
                    a = stack.pop()
                    if token == '+':
                        stack.append(a + b)
                    elif token == '-':
                        stack.append(a - b)
                    elif token == '*':
                        stack.append(a * b)
                    elif token == '/':
                        stack.append(a / b)
                    elif token == '^':
                        stack.append(a ** b)

                # Inverse trig functions
                elif token in (inverseTrigFunctions):
                    a = stack.pop()
                    if token == 'asin':
                        a = (math.asin(a))
                    elif token == 'acos':
                        a = (math.acos(a))
                    elif token == 'atan':
                        a = (math.atan(a))
                    if settings["AngleUnit"] == "degrees":
                        a = math.degrees(a)
                    stack.append(a)

                # Trig functions
                elif token in (trigFunctions):
                    a = stack.pop()
                    if settings["AngleUnit"] == "degrees":
                        a = math.radians(a)
                    if token == 'sin':
                        stack.append(math.sin(a))
                    elif token == 'cos':
                        stack.append(math.cos(a))
                    elif token == 'tan':
                        stack.append(math.tan(a))

                # Log functions
                elif token in logFunctions:
                    a = stack.pop()
                    if token == 'ln':
                        stack.append(math.log(a))
                    elif token == 'log':
                        stack.append(math.log10(a))

                # Sqrt functions
                elif token in miscFunctions:
                    a = stack.pop()
                    if token == 'sqrt' or token == 'root' or token == '√':
                        stack.append(math.sqrt(a))
        return stack[0]

    postfix = ConvertPostfix(tokens)
    result = EvaluatePostfix(postfix)
    return result

# Calculates the indefinite integral
# Prerequisites: Numerical string starting with int(
# Returns: String
# Handles: Implicit multiplication
def Integral(expression):
    # Remove the 'int' prefix
    expression = expression.lstrip("int").strip()

    expression, integrand = CalculusPreformatter(expression)

    # Create the symbol for integration
    x = Symbol(integrand)

    return CalculusPostformatter(str(integrate(expression, x)))

# Calculates the derivative
# Prerequisites: Numerical string starting with d/dx()
# Returns: String
# Handles: Implicit multiplication
def Derivative(expression):
    # Remove the 'd/dx' prefix
    expression = expression.lstrip("d/dx").strip()

    expression, integrand = CalculusPreformatter(expression)

    # Create the symbol for integration
    x = Symbol(integrand)

    return CalculusPostformatter(str(diff(expression, x)))

# Creates a sympy compatible input string for derivatives, integrals
# Prerequisites: Numerical string
# Returns: String
# Handles: Implicit multiplication
def CalculusPreformatter(expression):
    # Replace '^' with '**' for power notation
    expression = expression.replace('^', '**')
    # Replace 'root' with 'sqrt'
    expression = expression.replace("root", "sqrt")
    # Remove spaces
    expression = expression.replace(' ', '')
    # Extract the integrand if specified, else default to 'x'
    expression, integrand = expression.split(',') if ',' in expression else (expression, 'x')
    integrand = integrand.rstrip(')')
    # Ensure parentheses are balanced
    if expression.count('(') > expression.count(')'):
        expression += ')' * (expression.count('(') - expression.count(')'))

    # Insert multiplication between digits and parentheses
    expression = re.sub(r'(\d+)([a-zA-Z(])', r'\1*\2', expression)
    # Insert multiplication between variables and parentheses, excluding function names
    function_names = r'(sin|cos|tan|asin|acos|atan|sqrt|ln|log)'
    expression = re.sub(r'([a-zA-Z])(?!(' + function_names + r'))\(', r'\1*(', expression)
    # Insert multiplication between variables and function names
    expression = re.sub(r'([a-zA-Z])(' + function_names + r')', r'\1*\2', expression)
    # Clean up any erroneous multiplication symbols before function names
    expression = re.sub(r'(\b' + function_names + r')\*\(', r'\1(', expression)

    # Final sanity check for formatting
    expression = sympify(expression)

    return expression, integrand

# Converts sympy compatible text into user-readable
# Prerequisites: Sympy output string
# Returns: String
# Handles: None
def CalculusPostformatter(result):
    # Replace '**' with '^' for the output
    return result.replace('**', '^')

# Formats the raw output to the setting specified by the user
# Prerequisites: Numerical string
# Returns: String
def FormatResult(result):
    if settings["ExactFraction"]:
        result = Fraction(result).limit_denominator()
        return str(result)
    elif settings["ScientificNotation"]:
        return f"{result:.{settings['Decimals']}e}"
    elif settings["AutoDecimals"]:
        return f"{result:.12f}".rstrip('0').rstrip('.')
    else:
        return f"{result:.{settings['Decimals']}f}".rstrip('0').rstrip('.')

if __name__ == "__main__":
    LoadSettings(settingsFile)
    expression = sys.stdin.readline().strip()
    result = Calculator(expression)
    print(result)
