# Serialization

Types used for document types must be serializable classes with parameterless constructors.
It is recommended to use POCO classes and to have no constructors at all for document types.

Behind the scenes `System.Text.Json` is used for serialization and this cannot be changed.