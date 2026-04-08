# AGENTS.md

## Cel
Ten plik definiuje zasady pracy dla agentow AI kodujacych w repozytorium MindStakes.
Kontekst projektu: Godot + C#, logika w `Source/`, dane i zasoby w `Resources/`.

## Przeglad Repozytorium
- Silnik: Godot (plik projektu: `project.godot`)
- Jezyk: C# (`MindStakes.csproj`, `MindStakes.sln`)
- Logika gry i dane: `Source/`
- Zasoby (pytania, kategorie): `Resources/`
- Assety (audio, obrazy, motywy): `Assets/`

## Twarde Zasady
1. **Modyfikacja plikow `.tscn` jest zabroniona.**
2. Odczyt i analiza plikow `.tscn` sa dozwolone i wskazane.
3. Jesli zadanie wymaga zmian sceny, agent nie edytuje pliku sceny bezposrednio.
4. Zamiast tego agent opisuje kroki do wykonania recznie w edytorze Godot.

## Jak Opisywac Zmiany Scen
Gdy potrzebna jest zmiana sceny, podawaj instrukcje w tym formacie:
- Sciezka sceny (np. `Source/Screens/Creator/CreatorScreen.tscn`).
- Sciezka wezla do zaznaczenia w Godot.
- Akcja do wykonania (dodanie wezla, zmiana property, podpiecie sygnalu, kolejnosc wezlow itp.).
- Dokladne nazwy wlasciwosci i wartosci.
- Informacja o polach eksportowanych, ktore trzeba przypisac w Inspectorze.

Przykladowa struktura:
- Otworz scene: `...`
- Zaznacz wezel: `...`
- Dodaj child node: `...`
- Ustaw property: `... = ...`
- Podlacz sygnal: `... -> ...`
- Zapisz scene.

## Zakres Zmian W Kodzie
- Preferuj implementacje logiki w skryptach C# w `Source/`.
- Zachowuj obecna architekture i konwencje nazewnictwa.
- Unikaj refaktorow niezwiązanych z zadaniem.
- Nie zmieniaj zachowania publicznego, jesli nie wymaga tego zadanie.

## Dane i Zasoby
- Pytania sa przechowywane w `Resources/Questions/Pool[n]/`.
- Kategorie sa przechowywane w `Resources/Categories/`.
- Zachowuj konwencje plikow `.tres` uzywane w repozytorium.

## Walidacja
- Dla zmian C# uruchamiaj: `dotnet build MindStakes.sln`.
- Jesli build nie zostal uruchomiony, zaznacz to jawnie w odpowiedzi.

## Styl Komunikacji Agenta
- Odpowiedzi maja byc konkretne i skupione na implementacji.
- Przy zmianach scen zawsze zwracaj instrukcje do Godot Editor zamiast diffow dla `.tscn`.
