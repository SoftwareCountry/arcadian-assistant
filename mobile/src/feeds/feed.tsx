import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image } from 'react-native';
import { LayoutEvent } from 'react-navigation';
import moment from 'moment';

import { Avatar } from '../people/avatar';
import { Feed } from '../reducers/organization/feed.model';
import { feedStyles as styles } from './styles';
import { Employee } from '../reducers/organization/employee.model';

interface FeedListItemProps {
    message: Feed;
    employee: Employee;
    id: string;
}

interface FeedListItemState {
    imgContainerSize?: number;
    formattedDate?: string;
}

export class FeedListItem extends React.Component<FeedListItemProps, FeedListItemState> {
    constructor(props: FeedListItemProps, context: any) {
        super(props, context);
        this.state = {};
    }

    public componentWillReceiveProps(nextProps: Readonly<FeedListItemProps>, nextContent: any) {
        const nextDate = nextProps.message ? nextProps.message.datePosted : null;
        const datePosted = this.props.message ? this.props.message.datePosted : null;

        if (datePosted !== nextDate || !this.state.formattedDate) {
            this.setState({
                formattedDate: moment(nextDate).format('MMMM D, YYYY')
            });
        }
    }

    public render() {
        const message = this.props.message;
        const employee = this.props.employee;

        const imgStyle = StyleSheet.flatten([{
            width: this.state.imgContainerSize,
            height: this.state.imgContainerSize
        }]);

        const employeeName = employee ? employee.name : message.title;
        const photo = employee ? employee.photo : null;
        const mimeType = photo ? photo.mimeType : null;
        const base64 = photo ? photo.base64 : null;

        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <View style={styles.imgContainer}>
                        <Avatar mimeType={mimeType} photoBase64={base64} />
                    </View>
                    <View style={styles.info}>
                        <Text style={styles.title}>{employeeName}</Text>
                        <Text style={styles.text}>{this.props.message.text}</Text>
                        <Text style={styles.tags}>#ArcadiaNews</Text>
                        <Text style={styles.date}>{this.state.formattedDate}</Text>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}