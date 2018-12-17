import { Nullable } from 'types';

interface DaysCounter {
    title: [string, string];
    days: Nullable<number>;

    toString(): string;
}

export class VacationDaysCounter implements DaysCounter {
    public readonly title: [string, string] = ['days', 'of vacation left'];

    constructor(public readonly days: Nullable<number>) {
    }

    public toString() {
        if (this.days == null) {
            return '';
        }

        return this.days.toString();
    }
}

export enum HoursCreditType {
    Workout = 'Workout',
    DaysOff = 'DaysOff'
}

export class HoursCreditCounter implements DaysCounter {

    constructor(
        public readonly hours: Nullable<number>,
        public readonly days: Nullable<number>,
        public readonly rest: Nullable<string>
    ) {
    }

    public toString() {
        if (this.hours == null) {
            return '';
        }

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
        return this.hours && this.hours > 0
            ? HoursCreditType.Workout
            : HoursCreditType.DaysOff;
    }
}
