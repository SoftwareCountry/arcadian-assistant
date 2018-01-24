import { dataMember, required } from 'santee-dcts';

// TODO: mock models

export class DaysCounterItemRaw {
    @dataMember()
    @required()
    public timestamp: number;

    @dataMember()
    @required()
    public title: string;

    @dataMember()
    @required()
    public return: boolean;
}

export class DaysCounterRaw {
    @dataMember()
    @required()
    public allVacationDays: DaysCounterItemRaw;

    @dataMember()
    @required()
    public daysOff: DaysCounterItemRaw;
}

export class DaysCounterModelItem {
    public days: string;
    public title: string;
    public return: boolean;
}

export class DaysCountersModel {
    public allVacationDays: DaysCounterModelItem;
    public daysOff: DaysCounterModelItem;
}