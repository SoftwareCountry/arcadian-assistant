import { dataMember, required } from 'santee-dcts';
import { ConvertHoursCreditToDays } from './days-counters-calculator';

export class VacationDaysCounter {
    public readonly title = 'days of vacation left';

    constructor(public readonly days: number) { }

    public toString() {
        return (this.days ? this.days : '').toString();
    }
}

enum HoursCreditType {
    WorkOut = 'WorkOut',
    DaysOff = 'DaysOff'
}

export class HoursCreditCounter {
    constructor(
        public readonly hours: number,
        public readonly days: number,
        public readonly rest: string
    ) { }

    public toString() {
        return `${(this.days ? this.days : '')}${this.rest}`;
    }

    public get isWorkOut(): boolean {
        return this.type === HoursCreditType.WorkOut;
    }

    public get title(): string {
        const phrases = {
            [HoursCreditType.WorkOut]: 'return',
            [HoursCreditType.DaysOff]: 'left'
        };

        return `dayoffs to ${phrases[this.type]}`;
    }

    private get type(): HoursCreditType {
        return this.hours > 0
            ? HoursCreditType.WorkOut
            : HoursCreditType.DaysOff;
    }
}

export class DaysCountersModel {
    public allVacationDays: VacationDaysCounter;
    public hoursCredit: HoursCreditCounter;
}