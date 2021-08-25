# Описание
Приложение с описанием аддонов и личным рейтингом в игре [Dead by Daylight](https://store.steampowered.com/app/381210/Dead_by_Daylight/)

# Установка

## Windows OS

### Способ 1
- Перейти в список [релизов](https://github.com/NishonovA/DeadByDaylight_Addons/releases)
- Выбрать необходимую версию
- Скачать папку Release
- Запустить файл DeadByDaylight_Addons.exe


### Способ 2

#### Требования
- Установлен [git](https://git-scm.com/download/win)
- Установлен [.NET 5.0](https://dotnet.microsoft.com/download)

#### Порядок действий
- Клонировать репозиторий `git clone https://github.com/NishonovA/DeadByDaylight_Addons.git`
- Собрать приложение: `publish DeadByDaylight_Addons/DeadByDaylight_Addons.sln -o DbDAddons`
- В папке DbDAddons запустить файл DeadByDaylight_Addons.exe `start /d DbDAddons DeadByDaylight_Addons.exe`

```
git clone https://github.com/NishonovA/DeadByDaylight_Addons.git
publish DeadByDaylight_Addons/DeadByDaylight_Addons.sln -o DbDAddons
start /d DbDAddons DeadByDaylight_Addons.exe
```
