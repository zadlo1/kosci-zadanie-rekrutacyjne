# 🎲 Gra w Kości (Yahtzee)

> Konsolowa gra dla 1–4 graczy (ludzkich i/lub AI) zaimplementowana w **C# / .NET 9**, oparta na zasadach klasycznej gry Yahtzee.

---

## Spis treści

1. [Uruchomienie](#uruchomienie)
2. [Zasady gry](#zasady-gry)
3. [System punktacji](#system-punktacji)
4. [Decyzje projektowe](#decyzje-projektowe)
5. [Architektura](#architektura)
6. [Gracz AI](#gracz-ai)
7. [Testy](#testy)

---

## Uruchomienie

**Wymagania:** .NET 9 SDK ([pobierz](https://dotnet.microsoft.com/download/dotnet/9.0))

```bash
# Sklonuj repozytorium
git clone <url>
cd kosci-zadanie-rekrutacyjne

# Uruchom grę
cd GraWKosci
dotnet run

# Uruchom testy jednostkowe
cd GraWKosci.Tests
dotnet test
```

Po uruchomieniu gra pyta o liczbę graczy ludzkich (1–4), a następnie o liczbę graczy AI. Łączna liczba uczestników musi wynosić co najmniej 2.

---

## Zasady gry

### Przebieg tury

```
Rzut 1 (obowiązkowy) – gracz rzuca wszystkimi 5 kośćmi
         ↓
Wybór kości do zatrzymania (numery oddzielone spacją) lub Enter – rzut wszystkimi
         ↓
Rzut 2 (opcjonalny) – ponowny rzut niezatrzymanymi kośćmi
         ↓
Wybór kości do zatrzymania
         ↓
Rzut 3 (opcjonalny) – ostatni rzut niezatrzymanymi kośćmi
         ↓
Obowiązkowy wybór kategorii → zapis punktów
```

**Ważne zasady:**
- Każdą kategorię można użyć **tylko raz**.
- Po zakończeniu rzutów gracz **musi** wybrać kategorię — nawet jeśli żadna kombinacja nie pasuje (wynik = 0 pkt).
- Gracz może wybrać kategorię po 1. lub 2. rzucie, kończąc turę wcześniej.

### Interfejs konsolowy

```
=== Tura gracza: Michał ===

[1:4 ] [2:3 ] [3:4 ] [4:4 ] [5:1 ]

Pozostałe rzuty: 2
Zatrzymać kości? Podaj numery (np. 1 3 5) lub Enter, aby rzucić wszystkimi:
> 1 3 4

[1:4*] [2:6 ] [3:4*] [4:4*] [5:2 ]

=== PODPOWIEDZI (MOŻLIWE WYNIKI) ===
Fours -> 12 pkt
ThreeOfAKind -> 20 pkt
Chance -> 20 pkt
...

Wybierz kategorię (wpisz nazwę):
> ThreeOfAKind
Zapisano: ThreeOfAKind -> 20 pkt
```

Kości oznaczone `*` są zatrzymane. Podpowiedzi pokazują wyłącznie kategorie jeszcze niewykorzystane.

---

## System punktacji

### Sekcja górna

Punkty liczone jako suma wyrzuconych kości danej wartości.

| Kategoria | Opis                     | Przykład `[4,3,4,4,1]` |
|-----------|--------------------------|------------------------|
| Ones      | Suma jedynek             | 1 pkt                  |
| Twos      | Suma dwójek              | 0 pkt                  |
| Threes    | Suma trójek              | 3 pkt                  |
| Fours     | Suma czwórek             | 12 pkt                 |
| Fives     | Suma piątek              | 0 pkt                  |
| Sixes     | Suma szóstek             | 0 pkt                  |

**Premia:** suma sekcji górnej ≥ 63 pkt → jednorazowo **+35 pkt**.  
Próg 63 odpowiada trafieniu co najmniej 3 kości każdej wartości w każdej kategorii (3×1 + 3×2 + … + 3×6 = 63).

### Sekcja dolna

| Kategoria      | Warunek                                         | Punkty                  |
|----------------|-------------------------------------------------|-------------------------|
| ThreeOfAKind   | ≥ 3 jednakowe kości                             | Suma wszystkich 5 kości |
| FourOfAKind    | ≥ 4 jednakowe kości                             | Suma wszystkich 5 kości |
| FullHouse      | Trójka + para                                   | 25 pkt                  |
| SmallStraight  | 4 kolejne kości (1-2-3-4, 2-3-4-5 lub 3-4-5-6) | 30 pkt                  |
| LargeStraight  | 5 kolejnych kości (1-2-3-4-5 lub 2-3-4-5-6)    | 40 pkt                  |
| Yahtzee        | 5 jednakowych kości                             | 50 pkt                  |
| Chance         | Zawsze dostępna                                 | Suma wszystkich 5 kości |

---

## Decyzje projektowe

### Wybór technologii

Projekt implementuje C# / .NET 9. Język naturalnie wspiera wzorce obiektowe i funkcyjne (LINQ, pattern matching, wyrażenia switch), które dobrze pasują do domeny gry — obliczanie kombinacji, filtrowanie kategorii, mapowanie wartości. Brak zewnętrznych zależności poza xUnit upraszcza setup i eliminuje zbędny narzut.

### `int?` jako reprezentacja stanu karty

Kategorie w `ScoreCard` są przechowywane jako `int?` (nullable int). `null` oznacza kategorię **niezapisaną**, a `0` — kategorię **zapisaną z wynikiem zero**. To kluczowe rozróżnienie: gracz musi móc świadomie poświęcić kategorię (wpisać 0 pkt), a system musi to odróżnić od pola jeszcze nietkniętego. Alternatywą byłoby trzymanie osobnego słownika flag `bool[]`, ale model z nullable jest bardziej zwięzły i czytelny — sprawdzenie `Ones.HasValue` jest oczywiste.

### `ScoreCalculator` jako klasa statyczna

Logika obliczania i zapisywania wyników nie posiada stanu — te same kości i kategoria zawsze dają ten sam wynik. Klasa statyczna jednoznacznie wyraża tę bezstanowość, eliminuje potrzebę instancjowania i upraszcza testowanie (wywołania `ScoreCalculator.Calculate(...)` bez żadnego setup). Gdyby w przyszłości pojawiła się potrzeba wstrzykiwania (np. mockowanie w testach integracyjnych), wystarczyłoby wyekstrahować interfejs.

### Separacja modeli, serwisów i UI

Projekt dzieli kod na trzy warstwy:
- **Models** — czyste klasy danych bez logiki domenowej (`Dice`, `Player`, `ScoreCard`, `GameState`, `ScoreCategory`)
- **Services** — logika gry niezależna od sposobu prezentacji (`DiceRoller`, `ScoreCalculator`, `GameEngine`, `AiPlayer`)
- **UI** — renderowanie (`ConsoleRenderer`)

Dzięki temu logika gry jest w pełni testowalna bez uruchamiania konsoli, a ewentualna zamiana interfejsu konsolowego na graficzny wymaga pracy tylko w warstwie UI.

### `DiceRoller` jako klasa instancyjna

W odróżnieniu od `ScoreCalculator`, `DiceRoller` jest instancją — kapsułkuje obiekt `Random`. Pozwala to w przyszłości wstrzyknąć `Random` z seedem przez konstruktor i uzyskać deterministyczne rzuty w testach. Aktualne testy `DiceRollerTests` weryfikują zakres wartości (1–6) i niezmienność zatrzymanych kości bez potrzeby seedowania.

### `BonusApplied` jako flaga

Premia za sekcję górną (35 pkt przy ≥ 63 pkt) jest jednorazowa. Flaga `BonusApplied` w `ScoreCard` zapobiega wielokrotnemu naliczeniu premii, gdyby `TotalScore` był wywoływany wielokrotnie podczas jednej gry. `UpperBonus` zwraca 35 tylko gdy `!BonusApplied && UpperSectionTotal >= 63`; po przyznaniu premii `BonusApplied` jest ustawiane na `true`.

### Minimum 2 graczy

Gra wymaga co najmniej 2 uczestników — tryb jednoosobowy nie ma sensu bez rywalizacji i rankingu końcowego. Jeżeli gracz poda 1 gracza ludzkiego i 0 AI, program automatycznie dodaje drugiego gracza AI i informuje o tym użytkownika, zamiast zgłaszać błąd.

### Kolejność graczy

Gracze wykonują tury naprzemiennie w kolejności, w której zostali dodani (round-robin). Indeks aktualnego gracza jest przechowywany w `GameState.CurrentPlayerIndex` i inkrementowany modulo liczba graczy po każdej turze. `GameEngine` pomija graczy z ukończoną kartą, dzięki czemu gra kończy się dopiero gdy **wszyscy** ukończą 13 tur.

---

## Architektura

```
kosci-zadanie-rekrutacyjne/
├── kosci-zadanie-rekrutacyjne.sln
├── GraWKosci/
│   ├── Program.cs                   # Punkt wejścia: setup graczy, uruchomienie silnika
│   ├── Models/
│   │   ├── Dice.cs                  # Model kości: Value (1–6), IsHeld
│   │   ├── Player.cs                # Model gracza: Name, IsAi, ScoreCard
│   │   ├── ScoreCard.cs             # Karta wyników: 13 kategorii (int?), sumy, premia, IsComplete
│   │   ├── GameState.cs             # Stan gry: lista graczy, CurrentPlayerIndex, IsGameFinished
│   │   └── ScoreCategory.cs         # Enum 13 kategorii punktowych
│   ├── Services/
│   │   ├── DiceRoller.cs            # RollAll() i RollUnheld() — losowanie kości
│   │   ├── ScoreCalculator.cs       # Calculate() i Apply() — bezstanowa logika punktacji
│   │   ├── GameEngine.cs            # Główna pętla gry, tury ludzkie i AI, ranking końcowy
│   │   └── AiPlayer.cs              # Heurystyczna strategia AI
│   └── UI/
│       └── ConsoleRenderer.cs       # Renderowanie kości i podpowiedzi kategorii
└── GraWKosci.Tests/
    ├── ScoreCalculatorTests.cs
    ├── ScoreCardTests.cs
    ├── GameStateTests.cs
    ├── DiceRollerTests.cs
    └── AiPlayerTests.cs
```

---

## Gracz AI

AI podejmuje decyzje oparte na heurystykach zbliżonych do optymalnej strategii Yahtzee. Cała logika jest skupiona w klasie `AiPlayer` i podzielona na dwie metody publiczne.

### Wybór kości do zatrzymania — `ChooseDiceToHold`

Priorytety sprawdzane po kolei:

1. `rollsLeft == 0` → zatrzymaj wszystkie (koniec rzutów)
2. Duży strit → zatrzymaj wszystkie
3. Mały strit (kategoria wolna) → zatrzymaj cztery kości tworzące strit, rzuć piątą
4. Pięć jednaków → zatrzymaj wszystkie
5. Full → zatrzymaj wszystkie
6. Cztery jednakowe → zatrzymaj cztery
7. Trzy jednakowe → zatrzymaj trójkę
8. Dwie pary → zatrzymaj wyższą parę (z wyjątkami dla par jedynek i sekwencji)
9. Jedna para → zatrzymaj parę (z wyjątkami dla pary jedynek)
10. Brak par i stritów → zatrzymaj jedną kostkę z wartością 5, jeśli jest

### Wybór kategorii — `ChooseCategory`

Priorytety:

1. Yahtzee (5 jednakowych)
2. LargeStraight
3. SmallStraight
4. FullHouse
5. FourOfAKind → ewentualnie przekierowanie do odpowiedniej kategorii górnej
6. ThreeOfAKind → przy sumie ≥ 25 lub przekierowanie do sekcji górnej
7. Szansa przy słabych układach (suma ≥ 22 lub ≥ 20 zależnie od kontekstu)
8. Jedynki jako kategoria do "poświęcenia" przy słabych układach
9. Fallback → dostępna kategoria z najwyższym aktualnym wynikiem

---

## Testy

148 przypadków testowych w 5 klasach, pokrywających całą logikę domenową.  
Szczegółowy opis scenariuszy — zob. [TESTS.md](./TESTS.md).

| Plik | Przypadki testowe |
|------|-------------------|
| ScoreCalculatorTests | 65 |
| ScoreCardTests | 37 |
| AiPlayerTests | 27 |
| DiceRollerTests | 10 |
| GameStateTests | 9 |
| **Razem** | **148** |

```bash
cd GraWKosci.Tests
dotnet test
```
