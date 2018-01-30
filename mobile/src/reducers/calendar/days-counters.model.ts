import { dataMember, required } from 'santee-dcts';
import { ConvertHoursCreditToDays } from './convert-hours-credit-to-days';

interface DaysCounter {
    title: string;
    days: number;
    toString(): string;
}

export class VacationDaysCounter implements DaysCounter  {
    public readonly title = 'days of vacation left';

    constructor(public readonly days: number) { }

    public toString() {
        return this.days.toString();
    }
}

enum HoursCreditType {
    AdditionalWork = 'AdditionalWork',
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

    public get isAdditionalWork(): boolean {
        return this.type === HoursCreditType.AdditionalWork;
    }

    public get title(): string {
        const phrases = {
            [HoursCreditType.AdditionalWork]: 'return',
            [HoursCreditType.DaysOff]: 'left'
        };

        return `dayoffs to ${phrases[this.type]}`;
    }

    private get type(): HoursCreditType {
        return this.hours > 0
            ? HoursCreditType.AdditionalWork
            : HoursCreditType.DaysOff;
    }
}

export class DaysCountersModel {
    public allVacationDays: VacationDaysCounter;
    public hoursCredit: HoursCreditCounter;
}