import {dataMember, required} from 'santee-dcts';
import { DataMemberDecoratorParams } from 'santee-dcts/src/dataMemberDecorator';
import moment, { Moment } from 'moment';
import { Nullable } from 'types';

const datePostedDecoratorParams: DataMemberDecoratorParams  = {
    customDeserializer: (value: string) => moment(value)
};

export class Feed {

    @dataMember()
    @required()
    public messageId = '';

    @dataMember()
    @required({ nullable: true })
    public employeeId: Nullable<string> = null;

    @dataMember()
    @required()
    public title = '';

    @dataMember()
    @required()
    public text = '';

    @dataMember(datePostedDecoratorParams)
    @required()
    public datePosted: Moment = moment();
}
