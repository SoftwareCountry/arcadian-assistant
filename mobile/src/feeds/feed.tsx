import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image } from 'react-native';
import { LayoutEvent } from 'react-navigation';

import { Avatar } from '../people/avatar';
import { Feed } from '../reducers/feeds/feed.model';
import { feedStyles as styles } from './styles';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';

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

        const formattedDate = message.datePosted.format('MMMM D, YYYY');

        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <View style={styles.imgContainer}>
                        <Avatar photo={photo} />
                    </View>
                    <View style={styles.info}>
                        <StyledText style={styles.title}>{employeeName}</StyledText>
                        <StyledText>
                            <Text style={styles.to}>@Arcadians</Text>
                            <Text style={styles.text}>, {this.props.message.text}</Text>
                        </StyledText>
                        <StyledText style={styles.tags}>#ArcadiaNews</StyledText>
                        <StyledText style={styles.date}>{formattedDate}</StyledText>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}