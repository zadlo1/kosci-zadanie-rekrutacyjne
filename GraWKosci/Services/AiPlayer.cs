using GraWKosci.Models;

namespace GraWKosci.Services;

public class AiPlayer
{
    // --- WYBÓR KOŚCI DO ZATRZYMANIA ---

    public bool[] ChooseDiceToHold(Dice[] dice, int rollsLeft, ScoreCard card)
    {
        var values = dice.Select(d => d.Value).ToArray();
        var hold = new bool[5];

        // Jeśli to ostatni rzut - zatrzymaj wszystkie
        if (rollsLeft == 0)
        {
            for (int i = 0; i < 5; i++) hold[i] = true;
            return hold;
        }

        // Duży strit - zapisz od razu (zatrzymaj wszystkie)
        if (HasLargeStraight(values))
        {
            for (int i = 0; i < 5; i++) hold[i] = true;
            return hold;
        }

        // Mały strit - zatrzymaj cztery tworzące strit, rzuć piątą
        if (HasSmallStraight(values) && !card.SmallStraight.HasValue)
        {
            return HoldSmallStraight(values);
        }

        // Pięć jednaków
        if (HasOfAKind(values, 5))
        {
            for (int i = 0; i < 5; i++) hold[i] = true;
            return hold;
        }

        // Full - wyjątek: full z trzema jedynkami/dwójkami/trójkami -> zapisz jako Full od razu
        if (IsFullHouse(values))
        {
            for (int i = 0; i < 5; i++) hold[i] = true;
            return hold;
        }

        // Cztery jednakowe
        if (HasOfAKind(values, 4))
        {
            return HoldOfAKind(values, 4);
        }

        // Trzy jednakowe
        if (HasOfAKind(values, 3))
        {
            // Full z trzema jedynkami - zatrzymaj wszystkie (zostanie zapisane jako Full)
            var tripletVal = GetOfAKindValue(values, 3);

            if (rollsLeft == 1 && IsFullHouse(values))
            {
                for (int i = 0; i < 5; i++) hold[i] = true;
                return hold;
            }

            // Zostaw tylko trójkę, rzuć pozostałymi
            return HoldOfAKind(values, 3);
        }

        // Dwie pary - zostaw wyższą parę
        if (HasTwoPairs(values))
        {
            // Wyjątek przed 3. rzutem: para jedynek + para dwójek lub trójek -> zostaw obie
            if (rollsLeft == 1)
            {
                var pairs = GetPairValues(values);
                if (pairs.Contains(1) && (pairs.Contains(2) || pairs.Contains(3)))
                {
                    return HoldBothPairs(values);
                }

                // 11345 -> zostaw 345, nie parę
                if (pairs.Contains(1) && values.ToHashSet().IsSupersetOf(new[] { 3, 4, 5 }))
                {
                    return HoldValues(values, new[] { 3, 4, 5 });
                }

                // 12456 -> zostaw 456
                if (values.ToHashSet().IsSupersetOf(new[] { 1, 2, 4, 5, 6 }))
                {
                    return HoldValues(values, new[] { 4, 5, 6 });
                }
            }

            // Przed 2. rzutem:
            if (rollsLeft == 2)
            {
                var pairs = GetPairValues(values);

                // 12344 - zostaw parę (4,4), nie małego strita
                // (domyślnie zostawiamy wyższą parę, co obsługuje ten przypadek)

                // Para jedynek - zostaw kombinację 345 lub kostki 4, 5, 6
                if (pairs.Contains(1))
                {
                    var nonOnes = values.Where(v => v != 1).ToArray();
                    if (nonOnes.ToHashSet().IsSupersetOf(new[] { 3, 4, 5 }))
                        return HoldValues(values, new[] { 3, 4, 5 });

                    // Zostaw 5, 4 lub 6 spośród reszty
                    int bestKeep = nonOnes.Where(v => v == 5 || v == 6 || v == 4)
                                         .OrderByDescending(v => v)
                                         .FirstOrDefault();
                    if (bestKeep > 0)
                        return HoldValues(values, new[] { bestKeep });

                    // Brak dobrego wyboru - zostaw wyższą parę (jedynki są najsłabsze, wróć do ogólnej reguły)
                }

                // 3456 z parą - zostaw parę, nie małego strita
                if (values.ToHashSet().IsSupersetOf(new[] { 3, 4, 5, 6 }))
                {
                    int pairVal = pairs.Max();
                    return HoldValues(values, new[] { pairVal, pairVal });
                }
            }

            // Ogólna zasada: zostaw wyższą parę
            return HoldHigherPair(values);
        }

        // Jedna para
        if (HasPair(values))
        {
            var pairVal = GetPairValues(values).First();

            // Wyjątek: para jedynek + zachowanie 345 lub 5/4/6
            if (pairVal == 1)
            {
                var nonOnes = values.Where(v => v != 1).ToArray();
                if (nonOnes.ToHashSet().IsSupersetOf(new[] { 3, 4, 5 }))
                    return HoldValues(values, new[] { 3, 4, 5 });

                int bestKeep = nonOnes.Where(v => v == 5 || v == 6 || v == 4)
                                     .OrderByDescending(v => v)
                                     .FirstOrDefault();
                if (bestKeep > 0)
                    return HoldValues(values, new[] { bestKeep });
            }

            return HoldPair(values, pairVal);
        }

        // Wszystkie kości różne, brak stritów - zachowaj kostkę 5
        var hold5 = new bool[5];
        for (int i = 0; i < 5; i++)
            if (values[i] == 5) { hold5[i] = true; break; }
        return hold5;
    }

    // --- WYBÓR KATEGORII ---

    public ScoreCategory ChooseCategory(Dice[] dice, ScoreCard card)
    {
        var values = dice.Select(d => d.Value).ToArray();
        var available = GetAvailableCategories(card);

        // Kolejność preferencji zgodnie ze strategią
        // 1. Duże kombinacje (dolna tabela)
        if (!card.Yahtzee.HasValue && HasOfAKind(values, 5))
            return ScoreCategory.Yahtzee;

        if (!card.LargeStraight.HasValue && HasLargeStraight(values))
            return ScoreCategory.LargeStraight;

        if (!card.SmallStraight.HasValue && HasSmallStraight(values))
            return ScoreCategory.SmallStraight;

        if (!card.FullHouse.HasValue && IsFullHouse(values))
        {
            // Wyjątek: full z trzema jedynkami/dwójkami/trójkami zapisuj jako Full
            return ScoreCategory.FullHouse;
        }

        // 2. Trzy lub cztery jednakowe - górna tabela jeśli suma >= 25
        if (HasOfAKind(values, 4) && !card.FourOfAKind.HasValue)
        {
            // Cztery jednakowe nigdy nie powinny być użyte w pierwszej rundzie (wg strategii)
            // ale jeśli kategorii brakuje, zapisz
            int tripVal = GetOfAKindValue(values, 4);
            var upperCat = GetUpperCategory(tripVal);
            if (upperCat.HasValue && !card.IsUsed(upperCat.Value))
                return upperCat.Value;
            return ScoreCategory.FourOfAKind;
        }

        if (HasOfAKind(values, 3))
        {
            int tripVal = GetOfAKindValue(values, 3);
            int sum = values.Sum();

            if (sum >= 25 && !card.ThreeOfAKind.HasValue)
                return ScoreCategory.ThreeOfAKind;

            var upperCat = GetUpperCategory(tripVal);
            if (upperCat.HasValue && !card.IsUsed(upperCat.Value))
                return upperCat.Value;

            if (!card.ThreeOfAKind.HasValue)
                return ScoreCategory.ThreeOfAKind;
        }

        // 3. Słabe ręce: dwie pary, jedna para, wszystkie różne
        int totalSum = values.Sum();

        if (HasTwoPairs(values))
        {
            var pairs = GetPairValues(values);
            int lowerPair = pairs.Min();
            bool isLow = lowerPair <= 3;

            if (isLow)
            {
                if (totalSum >= 22 && !card.Chance.HasValue)
                    return ScoreCategory.Chance;

                var upperCat = GetUpperCategory(lowerPair);
                if (upperCat.HasValue && !card.IsUsed(upperCat.Value))
                    return upperCat.Value;
            }
            else
            {
                // Wysoka para - zapisz jedynki, chyba że suma >= 20
                // Wyjątek: 23446 -> Szansa mimo 19 pkt
                bool is23446 = values.OrderBy(v => v).SequenceEqual(new[] { 2, 3, 4, 4, 6 });
                if (is23446 && !card.Chance.HasValue)
                    return ScoreCategory.Chance;

                if (totalSum >= 20 && !card.Chance.HasValue)
                    return ScoreCategory.Chance;

                if (!card.Ones.HasValue)
                    return ScoreCategory.Ones;
            }
        }

        if (HasPair(values))
        {
            int pairVal = GetPairValues(values).First();
            bool isLow = pairVal <= 3;

            if (isLow)
            {
                if (totalSum >= 22 && !card.Chance.HasValue)
                    return ScoreCategory.Chance;
                var upperCat = GetUpperCategory(pairVal);
                if (upperCat.HasValue && !card.IsUsed(upperCat.Value))
                    return upperCat.Value;
            }
            else
            {
                if (totalSum >= 20 && !card.Chance.HasValue)
                    return ScoreCategory.Chance;
                if (!card.Ones.HasValue)
                    return ScoreCategory.Ones;
            }
        }

        // Wszystkie różne -> jedynki
        if (!card.Ones.HasValue)
            return ScoreCategory.Ones;

        // Fallback: wybierz dostępną kategorię z najwyższym wynikiem
        return available
            .OrderByDescending(cat => ScoreCalculator.Calculate(cat, dice))
            .First();
    }

    // --- HELPERY ---

    private static IEnumerable<ScoreCategory> GetAvailableCategories(ScoreCard card)
    {
        return Enum.GetValues<ScoreCategory>().Where(c => !card.IsUsed(c));
    }

    private static ScoreCategory? GetUpperCategory(int value) => value switch
    {
        1 => ScoreCategory.Ones,
        2 => ScoreCategory.Twos,
        3 => ScoreCategory.Threes,
        4 => ScoreCategory.Fours,
        5 => ScoreCategory.Fives,
        6 => ScoreCategory.Sixes,
        _ => null
    };

    private static bool HasOfAKind(int[] v, int n) =>
        v.GroupBy(x => x).Any(g => g.Count() >= n);

    private static int GetOfAKindValue(int[] v, int n) =>
        v.GroupBy(x => x).First(g => g.Count() >= n).Key;

    private static bool IsFullHouse(int[] v)
    {
        var counts = v.GroupBy(x => x).Select(g => g.Count()).OrderBy(x => x).ToArray();
        return counts.Length == 2 && counts[0] == 2 && counts[1] == 3;
    }

    private static bool HasSmallStraight(int[] v)
    {
        var set = v.ToHashSet();
        return set.IsSupersetOf(new[] { 1, 2, 3, 4 }) ||
               set.IsSupersetOf(new[] { 2, 3, 4, 5 }) ||
               set.IsSupersetOf(new[] { 3, 4, 5, 6 });
    }

    private static bool HasLargeStraight(int[] v)
    {
        var set = v.ToHashSet();
        return set.SetEquals(new[] { 1, 2, 3, 4, 5 }) ||
               set.SetEquals(new[] { 2, 3, 4, 5, 6 });
    }

    private static bool HasPair(int[] v) =>
        v.GroupBy(x => x).Any(g => g.Count() >= 2);

    private static bool HasTwoPairs(int[] v) =>
        v.GroupBy(x => x).Count(g => g.Count() >= 2) >= 2;

    private static List<int> GetPairValues(int[] v) =>
        v.GroupBy(x => x).Where(g => g.Count() >= 2).Select(g => g.Key).ToList();

    private static bool[] HoldOfAKind(int[] v, int n)
    {
        int target = GetOfAKindValue(v, n);
        var hold = new bool[5];
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            if (v[i] == target && count < n) { hold[i] = true; count++; }
        }
        return hold;
    }

    private static bool[] HoldHigherPair(int[] v)
    {
        int pairVal = GetPairValues(v).Max();
        return HoldPair(v, pairVal);
    }

    private static bool[] HoldBothPairs(int[] v)
    {
        var hold = new bool[5];
        var pairs = GetPairValues(v);
        var counted = new Dictionary<int, int>();
        for (int i = 0; i < 5; i++)
        {
            if (pairs.Contains(v[i]))
            {
                counted.TryGetValue(v[i], out int c);
                if (c < 2) { hold[i] = true; counted[v[i]] = c + 1; }
            }
        }
        return hold;
    }

    private static bool[] HoldPair(int[] v, int pairVal)
    {
        var hold = new bool[5];
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            if (v[i] == pairVal && count < 2) { hold[i] = true; count++; }
        }
        return hold;
    }

    private static bool[] HoldValues(int[] v, int[] toHold)
    {
        var hold = new bool[5];
        var remaining = toHold.ToList();
        for (int i = 0; i < 5; i++)
        {
            if (remaining.Contains(v[i]))
            {
                hold[i] = true;
                remaining.Remove(v[i]);
            }
        }
        return hold;
    }

    private static bool[] HoldSmallStraight(int[] v)
    {
        // Znajdź cztery tworzące mały strit
        int[][] straights = { new[] { 1, 2, 3, 4 }, new[] { 2, 3, 4, 5 }, new[] { 3, 4, 5, 6 } };
        var set = v.ToHashSet();
        foreach (var s in straights)
        {
            if (set.IsSupersetOf(s))
                return HoldValues(v, s);
        }
        return new bool[5];
    }
}