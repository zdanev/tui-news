## Coding Conventions for Gemini CLI Agent

- **Private Fields:** Do not start private fields with an underscore (`_`). Use `camelCase` for private fields.

- **File-Scoped Namespaces:** Use file-scoped namespaces (C# 10 and later) for all `.cs` files. This means declaring the namespace at the top of the file followed by a semicolon, without enclosing the code in curly braces.

- **Unused Using Clauses:** Always remove unused `using` clauses to keep the code clean and reduce unnecessary dependencies.

- **Blank Lines:** Leave one blank line between properties, methods, and other members within a class to improve readability.

- **Implicit Usings:** Utilize implicit `using` directives when possible to reduce boilerplate `using` statements in code files. Ensure `ImplicitUsings` is enabled in the project file.
