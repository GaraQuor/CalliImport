# Calli Import c#

An alternative of dllimport with calli

## Getting Started

Allows you to import native methods where you should normally use GetProcAddress, using syntax like dllimport

## Usage

  - Declare your import (declare your args in <>)
    ``` C#
    [CalliImport( "opengl32.dll", EntryPoint = "glClear")]
    public static CalliMethode<int> Clear;
    ```
  - Load native methodes
    ``` C#
    CalliImport.LoadMethods();
    ```

## Tips

  - Declare your import with a return (first args is the return type)
    ``` C#
    [CalliImport( "opengl32.dll", EntryPoint = "glClear")]
    public static CalliMethodeType<int, int> Clear;
    ```
  
  - Preload library (not methods)
    ``` C#
    CalliImport.LoadLibrary("opengl32.dll");
    ```
  
  - Load library specific imports
    ``` C#
    CalliImport.LoadMethods("opengl32.dll");
    ```
    
  - For a faster execute don't forget
    ``` C#
    [SuppressUnmanagedCodeSecurity]
    ```
  
## Authors

* **Trystan Delhaye** - [GaraQuor](https://github.com/GaraQuor)

## License

This project is licensed under the GNU General Public License v3.0 License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Thanks to Весьмой Шум (wholemy.ru)
