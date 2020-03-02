# PDBSharp
C# Library and Tools to read Program DataBase (PDB) files created by MSVC

The library supports reading 32 bit PDB files (also called "DS") as well as 16 bit PDB files (also called "JG").

Writing PDB files is not implemented yet. You will find some code around the repository, but it's not usable yet at this stage.

The main goal is to create a stable PDB Reader first, then write unit tests to ensure the reader works properly.
Writing these unit tests will lead to the creation of a corresponding writer to generate PDB files that the reader can work on

[![Build status](https://ci.appveyor.com/api/projects/status/phudbuu0pt3dg9yp/branch/master?svg=true)](https://ci.appveyor.com/project/smx-smx/pdbsharp/branch/master) [![Join the chat at https://gitter.im/smx-smx/PDBSharp](https://badges.gitter.im/smx-smx/PDBSharp.svg)](https://gitter.im/smx-smx/PDBSharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
