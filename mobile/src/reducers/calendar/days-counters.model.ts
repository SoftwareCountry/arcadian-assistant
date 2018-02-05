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

    public get title(): [string, string] {
        const phrases = {
            [HoursCreditType.AdditionalWork]: 'return',
            [HoursCreditType.DaysOff]: 'available'
        };

        return ['daysoff', phrases[this.type]];
    }

    private get type(): HoursCreditType {
        return this.hours > 0
            ? HoursCreditType.AdditionalWork
            : HoursCreditType.DaysOff;
    }
}

export class TodayCounter {
    public readonly day: string;
    public readonly month: string;

    constructor() {
        const currentDate = moment();

        this.day = currentDate.format('D');
        this.month = currentDate.format('MMMM');
    }
}

export class DaysCountersModel {
    public allVacationDays: VacationDaysCounter;
    public hoursCredit: HoursCreditCounter;
}