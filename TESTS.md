# TESTS.md — Plan testów jednostkowych

Dokument opisuje scenariusze testowe zapewniające pełne pokrycie logiki aplikacji.  
Testy pisane z użyciem **xUnit** i **FluentAssertions** 

---

## Spis treści

1. [ScoreCalculatorTests](#scorecalculatortests)
2. [ScoreCardTests](#scorecardtests)
3. [GameStateTests](#gamestatetests)
4. [DiceRollerTests](#dicerollertests)
5. [AiPlayerTests — ChooseDiceToHold](#aiplayertests--choosedicetohold)
6. [AiPlayerTests — ChooseCategory](#aiplayertests--choosecategory)
7. [Struktura plików testowych](#struktura-plików-testowych)

---

## ScoreCalculatorTests

Klasa: `GraWKosci.Services.ScoreCalculator`  
Metody: `Calculate(ScoreCategory, Dice[])`, `Apply(ScoreCard, ScoreCategory, int)`

Metoda pomocnicza używana we wszystkich testach kalkulatora:
```csharp
private static Dice[] Dice(params int[] values) =>
    values.Select(v => new GraWKosci.Models.Dice { Value = v }).ToArray();
```

### Sekcja górna

| ID | Scenariusz | Wejście | Oczekiwany wynik |
|----|------------|---------|-----------------|
| SC01 | Ones — brak jedynek | `[2,3,4,5,6]` | `0` |
| SC02 | Ones — wszystkie jedynki | `[1,1,1,1,1]` | `5` |
| SC03 | Ones — kilka jedynek | `[1,2,1,3,4]` | `2` |
| SC04 | Twos — suma dwójek | `[2,2,3,4,5]` | `4` |
| SC05 | Threes — suma trójek | `[3,3,3,1,2]` | `9` |
| SC06 | Fours — suma czwórek | `[4,4,4,4,1]` | `16` |
| SC07 | Fives — suma piątek | `[5,5,1,2,3]` | `10` |
| SC08 | Sixes — suma szóstek | `[6,6,6,6,6]` | `30` |

### Trójka (ThreeOfAKind)

| ID | Scenariusz | Wejście | Oczekiwany wynik |
|----|------------|---------|-----------------|
| SC09 | Dokładnie trzy jednakowe | `[3,3,3,1,2]` | `12` |
| SC10 | Cztery jednakowe spełnia warunek trójki | `[4,4,4,4,1]` | `17` |
| SC11 | Pięć jednaków spełnia warunek trójki | `[6,6,6,6,6]` | `30` |
| SC12 | Dwie pary — brak trójki | `[2,2,3,3,4]` | `0` |
| SC13 | Brak powtórzeń | `[1,2,3,4,5]` | `0` |

### Czwórka (FourOfAKind)

| ID | Scenariusz | Wejście | Oczekiwany wynik |
|----|------------|---------|-----------------|
| SC14 | Dokładnie cztery jednakowe | `[5,5,5,5,2]` | `22` |
| SC15 | Pięć jednaków spełnia warunek czwórki | `[3,3,3,3,3]` | `15` |
| SC16 | Trzy jednakowe — brak czwórki | `[2,2,2,1,3]` | `0` |

### Full (FullHouse)

| ID | Scenariusz | Wejście | Oczekiwany wynik |
|----|------------|---------|-----------------|
| SC17 | Poprawny full (2+3) | `[1,1,2,2,2]` | `25` |
| SC18 | Poprawny full (3+2) | `[5,5,5,6,6]` | `25` |
| SC19 | Pięć jednaków — nie full | `[3,3,3,3,3]` | `0` |
| SC20 | Cztery jednakowe — nie full | `[4,4,4,4,1]` | `0` |
| SC21 | Trzy różne wartości — nie full | `[1,1,2,3,3]` | `0` |

### Mały strit (SmallStraight)

| ID | Scenariusz | Wejście | Oczekiwany wynik |
|----|------------|---------|-----------------|
| SC22 | Sekwencja 1-2-3-4 (z duplikatem) | `[1,2,3,4,4]` | `30` |
| SC23 | Sekwencja 2-3-4-5 | `[2,3,4,5,1]` | `30` |
| SC24 | Sekwencja 3-4-5-6 | `[3,4,5,6,2]` | `30` |
| SC25 | Duży strit zawiera mały strit | `[1,2,3,4,5]` | `30` |
| SC26 | Brak 4 kolejnych | `[1,2,3,5,6]` | `0` |
| SC27 | Pięć jednaków — brak strita | `[2,2,2,2,2]` | `0` |

### Duży strit (LargeStraight)

| ID | Scenariusz | Wejście | Oczekiwany wynik |
|----|------------|---------|-----------------|
| SC28 | Sekwencja 1-2-3-4-5 | `[1,2,3,4,5]` | `40` |
| SC29 | Sekwencja 2-3-4-5-6 | `[2,3,4,5,6]` | `40` |
| SC30 | Mały strit z duplikatem — nie duży | `[1,2,3,4,4]` | `0` |
| SC31 | Brak 5 kolejnych | `[1,2,3,4,6]` | `0` |

### Yahtzee

| ID | Scenariusz | Wejście | Oczekiwany wynik |
|----|------------|---------|-----------------|
| SC32 | Pięć jednaków | `[6,6,6,6,6]` | `50` |
| SC33 | Cztery jednakowe — nie Yahtzee | `[5,5,5,5,1]` | `0` |

### Szansa (Chance)

| ID | Scenariusz | Wejście | Oczekiwany wynik |
|----|------------|---------|-----------------|
| SC34 | Suma wszystkich kości | `[1,2,3,4,5]` | `15` |
| SC35 | Maksymalna suma | `[6,6,6,6,6]` | `30` |
| SC36 | Minimalna suma | `[1,1,1,1,1]` | `5` |

### Apply

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| SC37 | Apply(Ones, 3) ustawia `card.Ones == 3` | |
| SC38 | Apply(Yahtzee, 50) ustawia `card.Yahtzee == 50` | |
| SC39 | Apply każdej z 13 kategorii ustawia właściwość | (parametryzowany test po `ScoreCategory`) |

---

## ScoreCardTests

Klasa: `GraWKosci.Models.ScoreCard`

### UpperSectionTotal

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| SCC01 | Pusta karta — wynik 0 | `UpperSectionTotal == 0` |
| SCC02 | Wypełnione wszystkie 6 kategorii | suma poprawna |
| SCC03 | Częściowo wypełniona — null traktowane jako 0 | |

### UpperBonus

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| SCC04 | Suma < 63 — brak premii | `UpperBonus == 0` |
| SCC05 | Suma == 63 — premia przyznana | `UpperBonus == 35` |
| SCC06 | Suma > 63 — premia przyznana | `UpperBonus == 35` |
| SCC07 | `BonusApplied == true` — premia nie naliczana ponownie | `UpperBonus == 0` |

### TotalScore

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| SCC08 | Pusta karta — wynik 0 | |
| SCC09 | Suma górna + premia + suma dolna | poprawna arytmetyka |

### IsComplete

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| SCC10 | Pusta karta — `false` | |
| SCC11 | 12 z 13 kategorii wypełnione — `false` | |
| SCC12 | Wszystkie 13 kategorii wypełnione — `true` | |

### IsUsed

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| SCC13 | Kategoria null — `false` | |
| SCC14 | Kategoria z wartością 0 — `true` (zapisano wynik 0) | |
| SCC15 | Każda kategoria poprawnie raportuje stan | (parametryzowany test) |

---

## GameStateTests

Klasa: `GraWKosci.Models.GameState`

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| GS01 | `CurrentPlayer` zwraca gracza o indeksie `CurrentPlayerIndex` | |
| GS02 | `IsGameFinished` — wszyscy gracze ukończeni → `true` | |
| GS03 | `IsGameFinished` — jeden gracz nieukończony → `false` | |
| GS04 | `IsGameFinished` — pusta lista graczy → `true` (All na pustej kolekcji) | |

---

## DiceRollerTests

Klasa: `GraWKosci.Services.DiceRoller`

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| DR01 | `RollAll()` zwraca tablicę 5 kości | `Length == 5` |
| DR02 | `RollAll()` — wszystkie kości niezatrzymane | każda `IsHeld == false` |
| DR03 | `RollAll()` — wartości w zakresie 1–6 | każda wartość `>= 1 && <= 6` |
| DR04 | `RollUnheld()` — niezatrzymane kości mają nowe wartości (statystycznie) | |
| DR05 | `RollUnheld()` — zatrzymane kości nie zmieniają wartości | |
| DR06 | `RollUnheld()` — wszystkie kości zatrzymane → żadna nie zmienia wartości | |
| DR07 | `RollAll()` wywołane wiele razy — nie zwraca zawsze tej samej sekwencji | |

> **Uwaga do DR04/DR07:** Ponieważ wynik jest losowy, najprościej sprawdzić zakres wartości i `IsHeld`. Determinizm można wymusić przez wstrzyknięcie `Random` z seedem w konstruktorze (refaktor opcjonalny).

---

## AiPlayerTests — ChooseDiceToHold

Klasa: `GraWKosci.Services.AiPlayer`  
Metoda: `ChooseDiceToHold(Dice[], int rollsLeft, ScoreCard)`

Metoda pomocnicza:
```csharp
private static Dice[] D(params int[] v) =>
    v.Select(x => new Dice { Value = x }).ToArray();
private static ScoreCard EmptyCard() => new ScoreCard();
```

### Zatrzymanie przy specjalnych kombinacjach

| ID | Scenariusz | Kości | rollsLeft | Oczekiwanie |
|----|------------|-------|-----------|-------------|
| AI01 | Duży strit — zatrzymaj wszystkie | `[1,2,3,4,5]` | `2` | `[T,T,T,T,T]` |
| AI02 | Duży strit — zatrzymaj wszystkie | `[2,3,4,5,6]` | `1` | `[T,T,T,T,T]` |
| AI03 | Pięć jednaków — zatrzymaj wszystkie | `[3,3,3,3,3]` | `2` | `[T,T,T,T,T]` |
| AI04 | Full — zatrzymaj wszystkie | `[2,2,3,3,3]` | `2` | `[T,T,T,T,T]` |
| AI05 | Cztery jednakowe — zatrzymaj cztery | `[5,5,5,5,2]` | `1` | 4 × `T`, 1 × `F` |
| AI06 | Trzy jednakowe — zatrzymaj trójkę | `[4,4,4,1,2]` | `2` | 3 × `T`, 2 × `F` |
| AI07 | Mały strit (wolna kategoria) — zatrzymaj 4 | `[1,2,3,4,6]` | `2` | `1,2,3,4` zatrzymane |
| AI08 | Dwie pary — zostaw wyższą | `[2,2,5,5,3]` | `2` | para 5 zatrzymana |
| AI09 | Jedna para — zatrzymaj parę | `[3,3,1,4,6]` | `2` | dwie `3` zatrzymane |
| AI10 | Brak par/stritów, jest 5 — zatrzymaj 5 | `[1,2,3,4,5]` | `2` | jedna `5` zatrzymana (gdy brak strita) |
| AI11 | Ostatni rzut (rollsLeft=0) — zatrzymaj wszystkie | `[1,2,3,4,6]` | `0` | `[T,T,T,T,T]` |
| AI12 | Mały strit (kategoria zajęta) — nie zatrzymuj jako strit | `[1,2,3,4,6]` — `SmallStraight` zajęta | `2` | inne zachowanie niż AI07 |

### Wynik metody

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| AI13 | Zawsze zwraca tablicę 5 elementów | `Length == 5` |

---

## AiPlayerTests — ChooseCategory

Klasa: `GraWKosci.Services.AiPlayer`  
Metoda: `ChooseCategory(Dice[], ScoreCard)`

### Priorytety — kombinacje wysokopunktowe

| ID | Scenariusz | Kości | Stan karty | Oczekiwana kategoria |
|----|------------|-------|------------|----------------------|
| AC01 | Yahtzee dostępny | `[6,6,6,6,6]` | pusta | `Yahtzee` |
| AC02 | Yahtzee zajęty — fallback | `[6,6,6,6,6]` | Yahtzee zajęty | nie `Yahtzee` |
| AC03 | Duży strit dostępny | `[1,2,3,4,5]` | pusta | `LargeStraight` |
| AC04 | Mały strit dostępny | `[1,2,3,4,6]` | pusta | `SmallStraight` |
| AC05 | Full dostępny | `[1,1,2,2,2]` | pusta | `FullHouse` |

### Priorytety — słabe układy

| ID | Scenariusz | Kości | Stan karty | Oczekiwana kategoria |
|----|------------|-------|------------|----------------------|
| AC06 | Suma ≥ 22, Szansa wolna | `[5,5,4,4,6]` | pusta | `Chance` |
| AC07 | Para jedynek, wolne jedynki | `[1,1,2,3,4]` | pusta | `Ones` |
| AC08 | Brak par, wolne jedynki | `[1,2,3,4,6]` | pusta | `Ones` |

### Fallback

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| AC09 | Wszystkie preferencje zajęte — wybiera dostępną z najwyższym wynikiem | zwraca `ScoreCategory`, nie rzuca wyjątku |
| AC10 | Karta z jedną wolną kategorią — wybiera ją | |

### Własność: brak niedozwolonych kategorii

| ID | Scenariusz | Oczekiwanie |
|----|------------|-------------|
| AC11 | Wynik nigdy nie wskazuje zajętej kategorii | `!card.IsUsed(result)` |

---

## Struktura plików testowych

Proponowane rozmieszczenie klas testowych w projekcie `GraWKosci.Tests`:

```
GraWKosci.Tests/
├── ScoreCalculatorTests.cs   # SC01–SC39
├── ScoreCardTests.cs          # SCC01–SCC15
├── GameStateTests.cs          # GS01–GS04
├── DiceRollerTests.cs         # DR01–DR07
└── AiPlayerTests.cs           # AI01–AI13, AC01–AC11
```

### Przykładowy test (xUnit)

```csharp
public class ScoreCalculatorTests
{
    private static Dice[] Dice(params int[] values) =>
        values.Select(v => new GraWKosci.Models.Dice { Value = v }).ToArray();

    [Fact]
    public void Calculate_FullHouse_Returns25()
    {
        var dice = Dice(1, 1, 2, 2, 2);
        var result = ScoreCalculator.Calculate(ScoreCategory.FullHouse, dice);
        Assert.Equal(25, result);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 40)]
    [InlineData(new[] { 2, 3, 4, 5, 6 }, 40)]
    public void Calculate_LargeStraight_Returns40(int[] values, int expected)
    {
        var result = ScoreCalculator.Calculate(ScoreCategory.LargeStraight, Dice(values));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ScoreCategory.Ones, 3)]
    [InlineData(ScoreCategory.Yahtzee, 50)]
    [InlineData(ScoreCategory.Chance, 17)]
    public void Apply_SetsCorrectProperty(ScoreCategory category, int value)
    {
        var card = new ScoreCard();
        ScoreCalculator.Apply(card, category, value);
        Assert.True(card.IsUsed(category));
    }
}
```

### Przykładowy test AI (parametryzowany)

```csharp
public class AiPlayerTests
{
    private readonly AiPlayer _ai = new();

    private static Dice[] D(params int[] v) =>
        v.Select(x => new GraWKosci.Models.Dice { Value = x }).ToArray();

    [Fact]
    public void ChooseDiceToHold_LargeStraight_HoldsAll()
    {
        var held = _ai.ChooseDiceToHold(D(1, 2, 3, 4, 5), 2, new ScoreCard());
        Assert.All(held, h => Assert.True(h));
    }

    [Fact]
    public void ChooseCategory_Yahtzee_ReturnsYahtzee_WhenAvailable()
    {
        var category = _ai.ChooseCategory(D(6, 6, 6, 6, 6), new ScoreCard());
        Assert.Equal(ScoreCategory.Yahtzee, category);
    }

    [Fact]
    public void ChooseCategory_NeverReturnsUsedCategory()
    {
        var card = new ScoreCard();
        // Wypełnij wszystkie poza Chance
        ScoreCalculator.Apply(card, ScoreCategory.Ones, 0);
        ScoreCalculator.Apply(card, ScoreCategory.Twos, 0);
        ScoreCalculator.Apply(card, ScoreCategory.Threes, 0);
        ScoreCalculator.Apply(card, ScoreCategory.Fours, 0);
        ScoreCalculator.Apply(card, ScoreCategory.Fives, 0);
        ScoreCalculator.Apply(card, ScoreCategory.Sixes, 0);
        ScoreCalculator.Apply(card, ScoreCategory.ThreeOfAKind, 0);
        ScoreCalculator.Apply(card, ScoreCategory.FourOfAKind, 0);
        ScoreCalculator.Apply(card, ScoreCategory.FullHouse, 0);
        ScoreCalculator.Apply(card, ScoreCategory.SmallStraight, 0);
        ScoreCalculator.Apply(card, ScoreCategory.LargeStraight, 0);
        ScoreCalculator.Apply(card, ScoreCategory.Yahtzee, 0);

        var result = _ai.ChooseCategory(D(1, 2, 3, 4, 6), card);
        Assert.Equal(ScoreCategory.Chance, result);
    }
}
```