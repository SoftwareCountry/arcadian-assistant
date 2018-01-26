import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { UserState } from '../reducers/organization/user.reducer';

interface CalendarScreenProps {
    user: UserState;
}

export class CalendarScreenImpl extends Component<CalendarScreenProps> {
    private username: string;

    public componentWillReceiveProps() {
        this.username = this.props.user.employee ? this.props.user.employee.name : '';
    }

    public render() {
        return <View style={styles.container}>
            <Text>Calendar</Text>
            <Text>Username: {this.username}</Text>
        </View>;
    }
}

const mapStateToProps = (state: AppState): CalendarScreenProps => ({
    user: state.organization.user
});

export const CalendarScreen = connect(mapStateToProps)(CalendarScreenImpl);