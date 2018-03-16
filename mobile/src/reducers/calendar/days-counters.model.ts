import { dataMember, required } from 'santee-dcts';
import { ConvertHoursCreditToDays } from './convert-hours-credit-to-days';
import moment from 'moment';

interface DaysCounter {
    title: [string, string];
    days: number;
    toString(): string;
}

export class VacationDaysCounter implements DaysCounter  {
    public readonly title: [string, string] = ['days', 'of vacation left'];

    constructor(public readonly days: number) { }

    public toString() {
        return this.days.toString();
    }
}

enum HoursCreditType {
    Workout = 'Workout',
    DaysOff = 'DaysOff'
}

export class HoursCreditCounter implements DaysCounter {

    constructor(
        public readonly hours: number,
        public readonly days: number,
        public readonly rest: string
    ) { }

    public toString() {
        if (this.hours === 0) {
            return '0';
        }

        return `${(this.days ? this.days : '')}${this.rest}`;
    }

    public get isWorkout(): boolean {
        return this.type === HoursCreditType.Workout;
    }

    public get title(): [string, string] {
        const phrases = {
            [HoursCreditType.Workout]: 'return',
            [HoursCreditType.DaysOff]: 'available'
        };

        return ['daysoff', phrases[this.type]];
    }

    private get type(): HoursCreditType {
        return this.hours > 0
            ? HoursCreditType.Workout
            : HoursCreditType.DaysOff;
    }
}

export class DaysCountersModel {
    public allVacationDays: VacationDaysCounter;
    public hoursCredit: HoursCreditCounter;
}