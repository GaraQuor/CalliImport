# Calli Import c#

An alternative of dllimport with calli

## Getting Started

Allows you to import native methods or you should normally use GetProcAddress, using a dllimport-like syntax

## Usage

  - Declare your import
    ``` C#
    [CalliImport( "opengl32.dll", EntryPoint = "glBindBuffer")]
    public static CalliMethode<BufferTarget, int> CalliBindBuffer;
    ```
  - Load native methodes
    ``` C#
    CalliImport.LoadMethods();
    ```

## Authors

* **Trystan Delhaye** - [GaraQuor](https://github.com/GaraQuor)

## License

This project is licensed under the GNU General Public License v3.0 License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Thanks to Весьмой Шум (wholemy.ru)
