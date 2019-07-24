import { Nullable } from 'types';

export class ConvertHoursCreditToDays {
    public static readonly halfDay = 4;
    public static readonly fractionSymbol = '½';

    public convert(hours: Nullable<number>): ConvertHoursCreditResult {
        if (hours == null) {
            return {
                days: null,
                rest: null
            };
        }

        hours = Math.abs(hours);

        if (hours === 0) {
            return {
                days: 0,
                rest: ''
            };
        }

        if (hours <= ConvertHoursCreditToDays.halfDay) {
            return {
                days: 0,
                rest: ConvertHoursCreditToDays.fractionSymbol
            };
        }

        const halves = hours / ConvertHoursCreditToDays.halfDay;
        const days = halves / 2;
        let [entireDays, rest] = this.splitDays(days);

        let fraction = '';
        if (!isNaN(rest)) {
            if (rest > 0.5) {
                ++entireDays;
            } else {
                fraction = ConvertHoursCreditToDays.fractionSymbol;
            }
        }

        return { days: entireDays, rest: fraction };
    }

    private splitDays(days: number): [number, number] {
        const [entire, rest] = days.toString().split('.');
        return [Number(entire), Number(`0.${rest}`)];
    }
}

interface ConvertHoursCreditResult {
    days: Nullable<number>;
    rest: Nullable<string>;
}
