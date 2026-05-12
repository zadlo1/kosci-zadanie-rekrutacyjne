# 🎲 Gra w Kości (Yahtzee)

> Konsolowa gra planszowa dla 1–4 graczy (ludzkich i/lub AI) zaimplementowana w **C# / .NET 9**, oparta na zasadach klasycznej gry Yahtzee.

---

## Spis treści

1. [Opis gry](#opis-gry)
2. [Uruchomienie](#uruchomienie)
3. [Zasady gry](#zasady-gry)
4. [System punktacji](#system-punktacji)
5. [Architektura projektu](#architektura-projektu)
6. [Gracz AI](#gracz-ai)
7. [Interfejs użytkownika](#interfejs-użytkownika)
8. [Testy](#testy)

---

## Opis gry

Gra w kości to turowa gra logiczno-losowa. Każdy gracz stara się uzyskać jak największą liczbę punktów, rzucając pięcioma klasycznymi kośćmi sześciennymi i wybierając optymalne kategorie punktowania. Gra kończy się, gdy wszyscy gracze zapełnią swoje karty wyników (13 kategorii na gracza).

Obsługiwane tryby:
- **PvP** – od 2 do 4 graczy ludzkich
- **PvAI** – gracze ludzcy z graczami AI (łącznie 2–4)
- **AI vs AI** – wyłącznie gracze sterowani przez AI

---

## Uruchomienie

**Wymagania:** .NET 9 SDK

```bash
# Sklonuj repozytorium
git clone <url>

# Uruchom grę
cd GraWKosci
dotnet run

# Uruchom testy
cd GraWKosci.Tests
dotnet test
```

Na starcie gra pyta o liczbę graczy ludzkich (1–4), a następnie o liczbę graczy AI. Łączna liczba uczestników musi wynosić co najmniej 2.

---

## Zasady gry

### Przebieg tury

```
Rzut 1 (obowiązkowy): gracz rzuca wszystkimi 5 kośćmi
         ↓
Wybór kości do zatrzymania (lub Enter — rzut wszystkimi)
         ↓
Rzut 2 (opcjonalny): ponowny rzut niezatrzymanymi kośćmi
         ↓
Wybór kości do zatrzymania
         ↓
Rzut 3 (opcjonalny): ostatni rzut niezatrzymanymi kośćmi
         ↓
Obowiązkowy wybór kategorii z karty wyników → zapis punktów
```

**Ważne zasady:**
- Gracz **zawsze musi** wybrać kategorię po zakończeniu serii rzutów, nawet jeśli żadna kombinacja nie pasuje (wynik = 0 pkt).
- Każdą kategorię można użyć **tylko raz**.
- Gracz może zakończyć serię przed 3. rzutem — wybierając kategorię po 1. lub 2. rzucie.

---

## System punktacji

### Sekcja górna (Tabelka 1)

Punkty liczone jako **suma wyrzuconych kości** danej wartości.

| Kategoria | Opis                        | Przykład (4, 3, 4, 4, 1) |
|-----------|-----------------------------|--------------------------|
| Jedynki   | Suma wyrzuconych jedynek    | 1 pkt                    |
| Dwójki    | Suma wyrzuconych dwójek     | 0 pkt                    |
| Trójki    | Suma wyrzuconych trójek     | 3 pkt                    |
| Czwórki   | Suma wyrzuconych czwórek    | 12 pkt                   |
| Piątki    | Suma wyrzuconych piątek     | 0 pkt                    |
| Szóstki   | Suma wyrzuconych szóstek    | 0 pkt                    |

**Premia:** jeśli suma sekcji górnej osiągnie ≥ 63 pkt, gracz otrzymuje jednorazowo **+35 pkt**.  
Próg 63 odpowiada uzyskaniu co najmniej trzech kości każdej wartości w każdej kategorii (3×1 + 3×2 + … + 3×6 = 63).

### Sekcja dolna (Tabelka 2)

| Kategoria      | Warunek                                              | Punkty                     |
|----------------|------------------------------------------------------|----------------------------|
| Trójka         | Co najmniej 3 jednakowe kości                        | Suma wszystkich 5 kości    |
| Czwórka        | Co najmniej 4 jednakowe kości                        | Suma wszystkich 5 kości    |
| Full           | Trójka + para                                        | 25 pkt                     |
| Mały strit     | 4 kolejne kości (1-2-3-4, 2-3-4-5 lub 3-4-5-6)      | 30 pkt                     |
| Duży strit     | 5 kolejnych kości (1-2-3-4-5 lub 2-3-4-5-6)         | 40 pkt                     |
| Król (Yahtzee) | 5 jednakowych kości                                  | 50 pkt                     |
| Szansa         | Zawsze dostępna                                      | Suma wszystkich 5 kości    |

---

## Architektura projektu

### Struktura katalogów

```
kosci-zadanie-rekrutacyjne/
├── kosci-zadanie-rekrutacyjne.sln
├── GraWKosci/
│   ├── Program.cs                   # Punkt wejścia: setup graczy, uruchomienie silnika
│   ├── Models/
│   │   ├── Dice.cs                  # Model kości (Value, IsHeld)
│   │   ├── Player.cs                # Model gracza (Name, IsAi, ScoreCard)
│   │   ├── ScoreCard.cs             # Karta wyników: 13 kategorii, sumy, premia, IsComplete
│   │   ├── GameState.cs             # Stan gry: lista graczy, aktualny gracz, IsGameFinished
│   │   └── ScoreCategory.cs         # Enum 13 kategorii punktowych
│   ├── Services/
│   │   ├── DiceRoller.cs            # RollAll() i RollUnheld() — losowanie kości
│   │   ├── ScoreCalculator.cs       # Calculate() i Apply() — statyczna logika punktacji
│   │   ├── GameEngine.cs            # Główna pętla gry, tury ludzkie i AI
│   │   └── AiPlayer.cs              # Strategia AI: wybór kości i kategorii
│   └── UI/
│       └── ConsoleRenderer.cs       # Renderowanie kości i podpowiedzi kategorii
└── GraWKosci.Tests/
    └── (testy — zob. TESTS.md)
```

### Kluczowe klasy

#### `Dice`
```csharp
public class Dice
{
    public int Value { get; set; }   // 1–6
    public bool IsHeld { get; set; } // czy zatrzymana między rzutami
}
```

#### `ScoreCard`
Przechowuje wartości 13 kategorii jako `int?` (null = niewypełniona). Udostępnia właściwości:
- `UpperSectionTotal` — suma sekcji górnej
- `UpperBonus` — 35 jeśli `UpperSectionTotal >= 63` i premia jeszcze nie przyznana, inaczej 0
- `LowerSectionTotal` — suma sekcji dolnej
- `TotalScore` — łączny wynik
- `IsComplete` — true gdy wszystkie 13 kategorii wypełnione
- `IsUsed(ScoreCategory)` — sprawdza, czy kategoria jest już zajęta
- `BonusApplied` — flaga zapobiegająca wielokrotnemu naliczeniu premii

#### `ScoreCalculator` (statyczny)
- `Calculate(ScoreCategory, Dice[])` — zwraca punkty dla danej kombinacji i kategorii
- `Apply(ScoreCard, ScoreCategory, int)` — zapisuje wynik do karty

#### `GameEngine`
Zarządza pętlą gry (`Run()`), obsługuje tury graczy ludzkich (`PlayHumanTurn`) i AI (`PlayAiTurn`), przyjmuje wybór kategorii i pilnuje kolejności graczy. Po zakończeniu gry wyświetla ranking.

#### `AiPlayer`
- `ChooseDiceToHold(Dice[], int rollsLeft, ScoreCard)` — zwraca `bool[5]` — które kości zatrzymać
- `ChooseCategory(Dice[], ScoreCard)` — zwraca `ScoreCategory` do zapisu

#### `DiceRoller`
- `RollAll()` — tworzy 5 nowych, niezatrzymanych kości z losowymi wartościami
- `RollUnheld(Dice[])` — ponownie rzuca kośćmi z `IsHeld == false`

---

## Gracz AI

AI podejmuje decyzje oparte na heurystykach zbliżonych do optymalnej strategii Yahtzee.

### Wybór kości do zatrzymania (priorytety)

1. Duży strit → zatrzymaj wszystkie
2. Mały strit (kategoria wolna) → zatrzymaj cztery kości tworzące strit
3. Pięć jednaków → zatrzymaj wszystkie
4. Full → zatrzymaj wszystkie
5. Cztery jednakowe → zatrzymaj cztery
6. Trzy jednakowe → zatrzymaj trójkę (lub wszystkie jeśli przed 3. rzutem i to full)
7. Dwie pary → zostaw wyższą parę (z wyjątkami dla niskich par + sekwencji)
8. Para → zostaw parę (z wyjątkami dla pary jedynek)
9. Brak par ani strita → zostaw kostkę o wartości 5 (jeśli jest)

### Wybór kategorii (priorytety)

1. Yahtzee (5 jednaków)
2. Duży strit
3. Mały strit
4. Full
5. Cztery/trzy jednakowe — ewentualnie przekierowanie do sekcji górnej
6. Szansa przy sumie ≥ 22 lub ≥ 20 (zależnie od układu)
7. Jedynki jako "śmietnik" dla słabych układów
8. Fallback: dostępna kategoria z najwyższym wynikiem

---

## Interfejs użytkownika

Gra działa w trybie konsolowym.

**Przykładowy widok tury:**

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

Kości oznaczone `*` są zatrzymane. Renderer wyświetla podpowiedzi tylko dla kategorii jeszcze niewykorzystanych.

**Końcowy ranking:**

```
==========================
       KONIEC GRY
==========================

1. Michał - 287 pkt <<< ZWYCIEZCA
2. AI-1 [AI] - 251 pkt
3. Kasia - 198 pkt
```

---

## Testy

Opis scenariuszy testowych — zob. [TESTS.md](./TESTS.md).