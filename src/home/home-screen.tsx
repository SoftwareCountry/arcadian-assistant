import React, { Component } from 'react';
import { View, Text, Button } from 'react-native';
import { NavigationScreenProps, NavigationActions } from 'react-navigation';
import Icon from 'react-native-vector-icons/Ionicons';
import styles from '../layout/styles';
import { connect, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

interface HomeScreenDispatchProps {
    onSicklistClick(): void;
    onHelpdeskClick(): void;
    onDaysoffClick(): void;
    onVacationsClick(): void;
}

const mapDispatchToProps = (dispatch: Dispatch<any>): HomeScreenDispatchProps => ({
    onSicklistClick: () => dispatch(NavigationActions.navigate( { routeName: 'Sicklist'} )),
    onHelpdeskClick: () => dispatch(NavigationActions.navigate( { routeName: 'Helpdesk'} )),
    onVacationsClick: () => dispatch(NavigationActions.navigate( { routeName: 'Vacations'} )),
    onDaysoffClick: () => dispatch(NavigationActions.navigate( { routeName: 'Daysoff'} )),
});

class HomeScreenImpl extends Component<HomeScreenDispatchProps> {

    public render() {

        return <View style={styles.container}>
            <Button title='Sicklist' onPress={this.props.onSicklistClick} />
            <Button title='Helpdesk' onPress={this.props.onHelpdeskClick} />
            <Button title='Daysoff' onPress={this.props.onDaysoffClick} />
            <Button title='Vacations' onPress={this.props.onVacationsClick} />
        </View>;
    }
}

export const HomeScreen = connect(undefined, mapDispatchToProps)(HomeScreenImpl);