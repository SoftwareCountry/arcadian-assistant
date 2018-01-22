import { dataMember, required } from 'santee-dcts';

// TODO: mock models

export class DaysCounterItem {
    @dataMember()
    @required()
    public leftDays: number;

    @dataMember()
    @required()
    public allDays: number;

    @dataMember()
    @required()
    public title: string;
}

export class DaysCountersModel {
    @dataMember()
    @required()    
    public vacation: DaysCounterItem;

    @dataMember()
    @required()    
    public off: DaysCounterItem;

    @dataMember()
    @required()    
    public sick: DaysCounterItem;
}