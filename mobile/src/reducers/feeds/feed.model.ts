import {dataMember, required} from 'santee-dcts';
import { DataMemberDecoratorParams } from 'santee-dcts/src/dataMemberDecorator';
import moment from 'moment';

const datePostedDecoratorParams: DataMemberDecoratorParams  = {
    customDeserializer: (value: string) => moment(value)
};

export class Feed {

    @dataMember()
    @required()
    public messageId: string;

    @dataMember()
    @required({ nullable: true })
    public employeeId: string | null;

    @dataMember()
    @required()
    public title: string;

    @dataMember()
    @required()
    public text: string;

    @dataMember(datePostedDecoratorParams)
    @required()
    public datePosted: moment.Moment;
}