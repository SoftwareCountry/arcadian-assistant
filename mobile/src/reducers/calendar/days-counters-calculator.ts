export class DaysCountersCalculator {
    public static readonly halfDay = 4;
    public static readonly fractionSymbol = '½';

    public calculateAllVacationDays(timestamp: number): string {
        const days = timestamp / (DaysCountersCalculator.halfDay * 2);

        return days.toString();
    }

    public calculateDaysOff(timestamp: number): string {
        if (timestamp === 0) {
            return '0';
        }
        if (timestamp <= DaysCountersCalculator.halfDay) {
            return DaysCountersCalculator.fractionSymbol;
        }

        const halfs = timestamp / DaysCountersCalculator.halfDay;
        const days = halfs / 2;
        let [entireDays, rest] = this.splitDays(days);

        let fraction = '';
        if (!isNaN(rest)) {
            if (rest > 0.5) {
                ++entireDays;
            } else {
                fraction = DaysCountersCalculator.fractionSymbol;
            }
        }

        return (entireDays ? entireDays.toString() : '') + fraction;
    }

    private splitDays(days: number): [number, number] {
        const [entire, frac] = days.toString().split('.');
        return [Number(entire), Number(`0.${frac}`)];
    }
}