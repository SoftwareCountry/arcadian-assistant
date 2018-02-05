import {dataMember, required} from 'santee-dcts';

export class Feed {

    @dataMember()
    @required()
    public messageId: string;

    @dataMember()
    @required()
    public employeeId: string;

    @dataMember()
    @required()
    public title: string;

    @dataMember()
    @required()
    public text: string;

    @dataMember()
    @required()
    public datePosted: string;
}