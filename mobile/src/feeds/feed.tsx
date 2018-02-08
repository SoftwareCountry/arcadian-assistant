import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image } from 'react-native';
import { LayoutEvent } from 'react-navigation';

import { Avatar } from '../people/avatar';
import { Feed } from '../reducers/feeds/feed.model';
import { feedStyles as styles } from './styles';
import { Employee } from '../reducers/organization/employee.model';
import { ArcadiaText } from '../override/acradia-text';

interface FeedListItemProps {
    message: Feed;
    employee: Employee;
}

export class FeedListItem extends React.Component<FeedListItemProps> {
    public render() {
        const message = this.props.message;
        const employee = this.props.employee;

        const employeeName = employee ? employee.name : message.title;
        const photo = employee ? employee.photo : null;
        const mimeType = photo ? photo.mimeType : null;
        const base64 = photo ? photo.base64 : null;

        const formattedDate = message.datePosted.format('MMMM D, YYYY');

        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <View style={styles.imgContainer}>
                        <Avatar mimeType={mimeType} photoBase64={base64} />
                    </View>
                    <View style={styles.info}>
                        <ArcadiaText style={styles.title}>{employeeName}</ArcadiaText>
                        <ArcadiaText>
                            <Text style={styles.to}>@Arcadians</Text>
                            <Text style={styles.text}>, {this.props.message.text}</Text>
                        </ArcadiaText>
                        <ArcadiaText style={styles.tags}>#ArcadiaNews</ArcadiaText>
                        <ArcadiaText style={styles.date}>{formattedDate}</ArcadiaText>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}