import { dataMember, required } from 'santee-dcts';

// TODO: mock models

export class DaysCounterItem {
    @dataMember()
    @required()
    public timestamp: number;

    @dataMember()
    @required()
    public title: string;
}

export class DaysCountersModel {
    @dataMember()
    @required()    
    public allVacationDays: DaysCounterItem;

    @dataMember()
    @required()    
    public daysOff: DaysCounterItem;
}