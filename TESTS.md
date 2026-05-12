# TESTS.md — Plan testów jednostkowych

Dokument opisuje scenariusze testowe zapewniające pełne pokrycie logiki aplikacji.  
Testy pisane z użyciem **xUnit**.

> **Łączna liczba przypadków testowych: 148**  
> (każdy `[InlineData]` przy `[Theory]` liczy się jako osobny przypadek)

| Plik | `[Fact]` | `[Theory]` | `[InlineData]` łącznie | Przypadki testowe |
|------|----------|------------|------------------------|-------------------|
| ScoreCalculatorTests | 34 | 6 | 31 | **65** |
| ScoreCardTests | 18 | 3 | 19 | **37** |
| GameStateTests | 9 | 0 | 0 | **9** |
| DiceRollerTests | 10 | 0 | 0 | **10** |
| AiPlayerTests | 23 | 2 | 4 | **27** |
| **Razem** | **94** | **11** | **54** | **148** |

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
**Łącznie: 65 przypadków testowych**

```csharp
private static Dice[] D(params int[] values) =>
    values.Select(v => new Dice { Value = v }).ToArray();
```

### Sekcja górna (10 `[Fact]`)

| ID | Nazwa testu | Wejście | Oczekiwany wynik |
|----|-------------|---------|-----------------|
| SC01 | `Ones_BrakJedynek_Zwraca0` | `[2,3,4,5,6]` | `0` |
| SC02 | `Ones_WszystkieJedynki_Zwraca5` | `[1,1,1,1,1]` | `5` |
| SC03 | `Ones_KilkaJedynek_ZwracaSume` | `[1,2,1,3,4]` | `2` |
| SC04 | `Twos_KilkaDwojek_ZwracaSume` | `[2,2,3,4,5]` | `4` |
| SC05 | `Twos_BrakDwojek_Zwraca0` | `[1,1,3,4,5]` | `0` |
| SC06 | `Threes_KilkaTrojek_ZwracaSume` | `[3,3,3,1,2]` | `9` |
| SC07 | `Fours_KilkaCzworek_ZwracaSume` | `[4,4,4,4,1]` | `16` |
| SC08 | `Fives_KilkaPiatek_ZwracaSume` | `[5,5,1,2,3]` | `10` |
| SC09 | `Sixes_WszystkieSzostki_Zwraca30` | `[6,6,6,6,6]` | `30` |
| SC10 | `Sixes_BrakSzostek_Zwraca0` | `[1,2,3,4,5]` | `0` |

### Trójka — ThreeOfAKind (6 `[Fact]`)

| ID | Nazwa testu | Wejście | Oczekiwany wynik |
|----|-------------|---------|-----------------|
| SC11 | `ThreeOfAKind_DokladnieTrzy_ZwracaSumeWszystkich` | `[3,3,3,1,2]` | `12` |
| SC12 | `ThreeOfAKind_CzteryJednakowe_SpelniaWarunekTrojki` | `[4,4,4,4,1]` | `17` |
| SC13 | `ThreeOfAKind_PiecJednakowychSpelniaWarunek` | `[6,6,6,6,6]` | `30` |
| SC14 | `ThreeOfAKind_DwiePary_Zwraca0` | `[2,2,3,3,4]` | `0` |
| SC15 | `ThreeOfAKind_WszystkieRozne_Zwraca0` | `[1,2,3,4,5]` | `0` |
| SC16 | `ThreeOfAKind_JednaPara_Zwraca0` | `[1,1,2,3,4]` | `0` |

### Czwórka — FourOfAKind (4 `[Fact]`)

| ID | Nazwa testu | Wejście | Oczekiwany wynik |
|----|-------------|---------|-----------------|
| SC17 | `FourOfAKind_DokladnieCztery_ZwracaSumeWszystkich` | `[5,5,5,5,2]` | `22` |
| SC18 | `FourOfAKind_PiecJednakowychSpelniaWarunek` | `[3,3,3,3,3]` | `15` |
| SC19 | `FourOfAKind_TrzyjJednakowe_Zwraca0` | `[2,2,2,1,3]` | `0` |
| SC20 | `FourOfAKind_BrakPowtorzen_Zwraca0` | `[1,2,3,4,5]` | `0` |

### Full — FullHouse (6 `[Fact]`)

| ID | Nazwa testu | Wejście | Oczekiwany wynik |
|----|-------------|---------|-----------------|
| SC21 | `FullHouse_TrojkaPlusPara_Zwraca25` | `[1,1,2,2,2]` | `25` |
| SC22 | `FullHouse_ParaPlusTrojka_Zwraca25` | `[5,5,5,6,6]` | `25` |
| SC23 | `FullHouse_PiecJednakowychNieJestFull_Zwraca0` | `[3,3,3,3,3]` | `0` |
| SC24 | `FullHouse_CzteryJednakoweBrakFull_Zwraca0` | `[4,4,4,4,1]` | `0` |
| SC25 | `FullHouse_TrzyRozneWartosci_Zwraca0` | `[1,1,2,3,3]` | `0` |
| SC26 | `FullHouse_WszystkieRozne_Zwraca0` | `[1,2,3,4,5]` | `0` |

### Mały strit — SmallStraight (1 `[Theory]` × 5 + 1 `[Theory]` × 4 = **9 przypadków**)

| ID | Nazwa testu | Wejście | Oczekiwany wynik |
|----|-------------|---------|-----------------|
| SC27 | `SmallStraight_PoprawnaKombinacja_Zwraca30` | `[1,2,3,4,4]` | `30` |
| SC28 | `SmallStraight_PoprawnaKombinacja_Zwraca30` | `[2,3,4,5,1]` | `30` |
| SC29 | `SmallStraight_PoprawnaKombinacja_Zwraca30` | `[3,4,5,6,2]` | `30` |
| SC30 | `SmallStraight_PoprawnaKombinacja_Zwraca30` | `[1,2,3,4,5]` | `30` |
| SC31 | `SmallStraight_PoprawnaKombinacja_Zwraca30` | `[2,3,4,5,6]` | `30` |
| SC32 | `SmallStraight_NiepoprawnaKombinacja_Zwraca0` | `[1,2,3,5,6]` | `0` |
| SC33 | `SmallStraight_NiepoprawnaKombinacja_Zwraca0` | `[2,2,2,2,2]` | `0` |
| SC34 | `SmallStraight_NiepoprawnaKombinacja_Zwraca0` | `[1,1,1,1,1]` | `0` |
| SC35 | `SmallStraight_NiepoprawnaKombinacja_Zwraca0` | `[1,3,5,2,6]` | `0` |

### Duży strit — LargeStraight (1 `[Theory]` × 3 + 1 `[Theory]` × 3 = **6 przypadków**)

| ID | Nazwa testu | Wejście | Oczekiwany wynik |
|----|-------------|---------|-----------------|
| SC36 | `LargeStraight_PoprawnaKombinacja_Zwraca40` | `[1,2,3,4,5]` | `40` |
| SC37 | `LargeStraight_PoprawnaKombinacja_Zwraca40` | `[2,3,4,5,6]` | `40` |
| SC38 | `LargeStraight_PoprawnaKombinacja_Zwraca40` | `[5,4,3,2,1]` | `40` |
| SC39 | `LargeStraight_NiepoprawnaKombinacja_Zwraca0` | `[1,2,3,4,4]` | `0` |
| SC40 | `LargeStraight_NiepoprawnaKombinacja_Zwraca0` | `[1,2,3,4,6]` | `0` |
| SC41 | `LargeStraight_NiepoprawnaKombinacja_Zwraca0` | `[1,1,1,1,1]` | `0` |

### Yahtzee (1 `[Theory]` × 3 + 2 `[Fact]` = **5 przypadków**)

| ID | Nazwa testu | Wejście | Oczekiwany wynik |
|----|-------------|---------|-----------------|
| SC42 | `Yahtzee_PiecJednakowychDowolnaWartosc_Zwraca50` | `[1,1,1,1,1]` | `50` |
| SC43 | `Yahtzee_PiecJednakowychDowolnaWartosc_Zwraca50` | `[3,3,3,3,3]` | `50` |
| SC44 | `Yahtzee_PiecJednakowychDowolnaWartosc_Zwraca50` | `[6,6,6,6,6]` | `50` |
| SC45 | `Yahtzee_CzteryJednakowe_Zwraca0` | `[5,5,5,5,1]` | `0` |
| SC46 | `Yahtzee_BrakPowtorzen_Zwraca0` | `[1,2,3,4,5]` | `0` |

### Szansa — Chance (3 `[Fact]`)

| ID | Nazwa testu | Wejście | Oczekiwany wynik |
|----|-------------|---------|-----------------|
| SC47 | `Chance_ZwracaSumeWszystkichKosci` | `[1,2,3,4,5]` | `15` |
| SC48 | `Chance_MaksymalnaSuma` | `[6,6,6,6,6]` | `30` |
| SC49 | `Chance_MinimalnaSuma` | `[1,1,1,1,1]` | `5` |

### Apply (1 `[Theory]` × 13 + 3 `[Fact]` = **16 przypadków**)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| SC50–SC62 | `Apply_KazdaKategoria_UstawiaWlasciwosc` (×13) | Apply dla każdej z 13 kategorii | `card.IsUsed(category) == true` |
| SC63 | `Apply_Ones_UstawiaPoprawnaWartosc` | `Apply(Ones, 3)` | `card.Ones == 3` |
| SC64 | `Apply_Yahtzee_UstawiaPoprawnaWartosc` | `Apply(Yahtzee, 50)` | `card.Yahtzee == 50` |
| SC65 | `Apply_Wynik0_KategoriaTraktowanaJakoUzyta` | `Apply(FullHouse, 0)` | `IsUsed == true`, `card.FullHouse == 0` |

---

## ScoreCardTests

Klasa: `GraWKosci.Models.ScoreCard`  
**Łącznie: 37 przypadków testowych**

### UpperSectionTotal (4 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| SCC01 | `UpperSectionTotal_PustaKarta_Zwraca0` | Pusta karta | `0` |
| SCC02 | `UpperSectionTotal_WszystkieWypelnione_ZwracaPoprawnaSume` | Ones=3…Sixes=18 | `63` |
| SCC03 | `UpperSectionTotal_CzesciowoWypelniona_NullTraktowaneJako0` | Ones=5, Threes=9 | `14` |
| SCC04 | `UpperSectionTotal_ZapisBrzegowy0_NieWliczaNulla` | Ones=0 (zapisane) | `0` |

### UpperBonus (5 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| SCC05 | `UpperBonus_SumaPonizej63_Zwraca0` | Ones=1, Twos=2 | `0` |
| SCC06 | `UpperBonus_SumaRowna63_Zwraca35` | Suma górna == 63 | `35` |
| SCC07 | `UpperBonus_SumaPowyżej63_Zwraca35` | Suma górna > 63 | `35` |
| SCC08 | `UpperBonus_BonusAppliedTrue_Zwraca0NawetGdySuma63` | Suma == 63, `BonusApplied = true` | `0` |
| SCC09 | `UpperBonus_Prog62_BrakPremii` | Suma górna == 62 | `0` |

### LowerSectionTotal (2 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| SCC10 | `LowerSectionTotal_PustaKarta_Zwraca0` | Pusta karta | `0` |
| SCC11 | `LowerSectionTotal_WszystkieWypelnione_ZwracaPoprawnaSume` | Wszystkie 7 pól dolnych wypełnionych | `206` |

### TotalScore (3 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| SCC12 | `TotalScore_PustaKarta_Zwraca0` | Pusta karta | `0` |
| SCC13 | `TotalScore_ZPremia_PoprawnaArytmetyka` | Górna=63 + premia=35 + Yahtzee=50 | `148` |
| SCC14 | `TotalScore_BezPremii_PoprawnaArytmetyka` | Ones=1, Chance=10 | `11` |

### IsComplete (4 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| SCC15 | `IsComplete_PustaKarta_FalseReturn` | Pusta karta | `false` |
| SCC16 | `IsComplete_12z13Kategorii_FalseReturn` | Brak Chance | `false` |
| SCC17 | `IsComplete_Wszystkie13Kategorii_TrueReturn` | Wszystkie 13 zapisane | `true` |
| SCC18 | `IsComplete_KategorieZWartoscia0SaWypelnione` | Wszystkie 13 zapisane z wartością `0` | `true` |

### IsUsed (3 `[Theory]` × 13 + 3 + 3 = **19 przypadków**)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| SCC19–SCC31 | `IsUsed_KategoriaNiezapisana_FalseReturn` (×13) | Nowa karta, każda z 13 kategorii | `false` |
| SCC32–SCC34 | `IsUsed_KategoriaZapisanaZWartosciaPozytywna_TrueReturn` (×3: Ones, Yahtzee, Chance) | `Apply` z wartością 10 | `true` |
| SCC35–SCC37 | `IsUsed_KategoriaZapisanaZ0_TrueReturn` (×3: Ones, FullHouse, Yahtzee) | `Apply` z wartością `0` | `true` |

---

## GameStateTests

Klasa: `GraWKosci.Models.GameState`  
**Łącznie: 9 przypadków testowych**

### CurrentPlayer (2 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| GS01 | `CurrentPlayer_IndeksDomyslny0_ZwracaPierwszegoGracza` | Indeks = 0 | zwraca p1 |
| GS02 | `CurrentPlayer_IndeksPrzesunienty_ZwracaWlasciwego` | Indeks = 1 | zwraca p2 |

### IsGameFinished (3 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| GS03 | `IsGameFinished_WszyscyGraczeUkonczeni_TrueReturn` | Wszyscy z pełną kartą | `true` |
| GS04 | `IsGameFinished_JedenGraczNieUkonczony_FalseReturn` | Jeden gracz z pustą kartą | `false` |
| GS05 | `IsGameFinished_WszyscyGraczeZPustaKarta_FalseReturn` | Wszyscy z pustą kartą | `false` |

### Players (2 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| GS06 | `Players_DomyslniePustaLista` | Nowy `GameState` | `Players` jest pusta |
| GS07 | `Players_MoznaDeodacGraczy` | Dodano 2 graczy | `Count == 2` |

### CurrentPlayerIndex (2 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| GS08 | `CurrentPlayerIndex_DomyslnieZero` | Nowy `GameState` | `0` |
| GS09 | `CurrentPlayerIndex_MoznaZmienic` | Ustawiono na 1 | `1` |

---

## DiceRollerTests

Klasa: `GraWKosci.Services.DiceRoller`  
**Łącznie: 10 przypadków testowych**

### RollAll (5 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| DR01 | `RollAll_ZwracaDokladnie5Kosci` | Wywołanie `RollAll()` | `Length == 5` |
| DR02 | `RollAll_WszystkieKosciNiezatrzymane` | Wywołanie `RollAll()` | każda `IsHeld == false` |
| DR03 | `RollAll_WartosciWZakresie1Do6` | Wywołanie `RollAll()` | każda wartość w zakresie 1–6 |
| DR04 | `RollAll_WielokrotneWywolania_WartosciWZakresie` | 100 wywołań | zawsze w zakresie 1–6 |
| DR05 | `RollAll_ZwracaNoweTablice_NieZwracaReferencjiDoTejSamej` | Dwa wywołania | `NotSame(dice1, dice2)` |

### RollUnheld (5 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| DR06 | `RollUnheld_ZatrzymaneKosciNieZmieniajWartosci` | Wszystkie zatrzymane z wartością 6 | każda nadal == 6 |
| DR07 | `RollUnheld_NiezatrzymaneKosciMajaWartoscWZakresie` | Wszystkie niezatrzymane, 50 powtórzeń | wartości w zakresie 1–6 |
| DR08 | `RollUnheld_CzescZatrzymana_CzescNie` | Kości 0, 2, 4 zatrzymane; 1, 3 nie | zatrzymane == 6; pozostałe w zakresie 1–6 |
| DR09 | `RollUnheld_WszystkieZatrzymane_ZadnaNieZmieniaSie` | Wszystkie 5 zatrzymane z wartościami 1–5 | każda zachowuje oryginalną wartość |
| DR10 | `RollUnheld_ZadnaZatrzymana_WszystkieOtrzymujaNowWartosci` | Wartość startowa 7 (niemożliwa), niezatrzymane | każda po rzucie w zakresie 1–6 |

---

## AiPlayerTests — ChooseDiceToHold

Klasa: `GraWKosci.Services.AiPlayer`  
Metoda: `ChooseDiceToHold(Dice[], int rollsLeft, ScoreCard)`  
**Łącznie: 13 przypadków testowych (11 `[Fact]` + 1 `[Theory]` × 2)**

### Rozmiar odpowiedzi (1 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| AI01 | `ChooseDiceToHold_ZawszeZwraca5Elementow` | Dowolne kości | `Length == 5` |

### rollsLeft == 0 (1 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| AI02 | `ChooseDiceToHold_OstatniRzut_ZatrzymujeWszystkie` | `rollsLeft = 0` | wszystkie `true` |

### Kombinacje (9 `[Fact]` + 1 `[Theory]` × 2)

| ID | Nazwa testu | Kości | rollsLeft | Oczekiwanie |
|----|-------------|-------|-----------|-------------|
| AI03a | `ChooseDiceToHold_DuzyStrit_ZatrzymujeWszystkie` | `[1,2,3,4,5]` | `2` | wszystkie `true` |
| AI03b | `ChooseDiceToHold_DuzyStrit_ZatrzymujeWszystkie` | `[2,3,4,5,6]` | `2` | wszystkie `true` |
| AI04 | `ChooseDiceToHold_PiecJednakowychWszystkie_ZatrzymujeWszystkie` | `[4,4,4,4,4]` | `2` | wszystkie `true` |
| AI05 | `ChooseDiceToHold_Full_ZatrzymujeWszystkie` | `[2,2,3,3,3]` | `2` | wszystkie `true` |
| AI06 | `ChooseDiceToHold_CzteryJednakowe_ZatrzymujeDokladnieCztery` | `[5,5,5,5,2]` | `2` | dokładnie 4 × `true` |
| AI07 | `ChooseDiceToHold_CzteryJednakowe_ZatrzymujeWartoscKtoraMaPowtorzenieNie2` | `[5,5,5,5,2]` | `2` | kość z wartością 2 ma `false` |
| AI08 | `ChooseDiceToHold_TrzyJednakowe_ZatrzymujeDokladnieTrzy` | `[4,4,4,1,2]` | `2` | dokładnie 3 × `true` |
| AI09 | `ChooseDiceToHold_MalyStritKategoriaWolna_ZatrzymujeCztery` | `[1,2,3,4,6]` | `2` | dokładnie 4 × `true` |
| AI10 | `ChooseDiceToHold_MalyStritKategoriaZajeta_NieWybieraMalego` | `[1,2,3,4,6]`, SmallStraight zajęty | `2` | wynik różny od wariantu z wolną kategorią |
| AI11 | `ChooseDiceToHold_DwiePary_ZatrzymujeCoNajmniej2Kostki` | `[2,2,5,5,3]` | `2` | `>= 2` × `true` |
| AI12 | `ChooseDiceToHold_JednaPara_ZatrzymujeDokladnieDwie` | `[3,3,1,4,6]` | `2` | dokładnie 2 × `true` |
| AI13 | `ChooseDiceToHold_BrakParBrak5_ZatrzymujeMaxJedna` | `[1,2,3,6,4]` | `2` | zwraca tablicę 5 elementów bez wyjątku |

---

## AiPlayerTests — ChooseCategory

Klasa: `GraWKosci.Services.AiPlayer`  
Metoda: `ChooseCategory(Dice[], ScoreCard)`  
**Łącznie: 14 przypadków testowych (12 `[Fact]` + 1 `[Theory]` × 2)**

### Wynik nigdy nie wskazuje zajętej kategorii (2 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| AC01 | `ChooseCategory_NigdyNieWybieraZajetejKategorii_Yahtzee` | Yahtzee zajęty, kości `[6,6,6,6,6]` | wynik ≠ `Yahtzee` |
| AC02 | `ChooseCategory_NigdyNieWybieraZajetejKategorii_KartaZJednaWolna` | Pełna karta poza Chance | `!card.IsUsed(result)` |

### Priorytety — kombinacje wysokopunktowe (4 `[Fact]` + 1 `[Theory]` × 2)

| ID | Nazwa testu | Kości | Stan karty | Oczekiwana kategoria |
|----|-------------|-------|------------|----------------------|
| AC03 | `ChooseCategory_Yahtzee_WybieraYahtzee` | `[6,6,6,6,6]` | pusta | `Yahtzee` |
| AC04a | `ChooseCategory_DuzyStrit_WybieraLargeStraight` | `[1,2,3,4,5]` | pusta | `LargeStraight` |
| AC04b | `ChooseCategory_DuzyStrit_WybieraLargeStraight` | `[2,3,4,5,6]` | pusta | `LargeStraight` |
| AC05 | `ChooseCategory_MalyStrit_WybieraSmallStraight` | `[1,2,3,4,6]` | pusta | `SmallStraight` |
| AC06 | `ChooseCategory_Full_WybieraFullHouse` | `[1,1,2,2,2]` | pusta | `FullHouse` |

### Fallback (2 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| AC07 | `ChooseCategory_WszystkiePreferencjeZajete_WybieraDostepna` | Wolna tylko Twos | `Twos` |
| AC08 | `ChooseCategory_PustaKarta_NieRzucaWyjatku` | Kości `[1,1,1,1,1]`, pusta karta | brak wyjątku |

### Wartości brzegowe / walidacja wynikowa (2 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| AC09 | `ChooseCategory_WszystkieKombiancjeTestedNieRzucajaWyjatku` | 10 kombinacji kości, pusta karta | brak wyjątku dla żadnej |
| AC10 | `ChooseCategory_ZwracaPoprawnaKategorieEnumValue` | Kości `[1,2,3,4,5]` | `Enum.IsDefined == true` |

### Spójność Choose → IsUsed (2 `[Fact]`)

| ID | Nazwa testu | Scenariusz | Oczekiwanie |
|----|-------------|------------|-------------|
| AC11 | `ChooseCategory_WynikMoznaZapisacDoKarty` | Wybrana kategoria → `Apply` → `IsUsed` | `IsUsed(chosen) == true` |
| AC12 | `ChooseCategory_PelnaChaotycznaGra_KartaKompletnaPoWszystkichRundach` | Symulacja 13 tur, deterministyczne kości | karta kompletna, AI nigdy nie wybiera zajętej kategorii |

---

## Struktura plików testowych

```
GraWKosci.Tests/
├── GraWKosci_Tests.csproj
├── ScoreCalculatorTests.cs   # SC01–SC65   (65 przypadków)
├── ScoreCardTests.cs         # SCC01–SCC37 (37 przypadków)
├── GameStateTests.cs         # GS01–GS09   ( 9 przypadków)
├── DiceRollerTests.cs        # DR01–DR10   (10 przypadków)
└── AiPlayerTests.cs          # AI01–AI13, AC01–AC12 (27 przypadków)
```
