import React, {Component} from 'react';
import {View} from 'react-native';
import styles from '../layout/styles';
import {Calendar} from './calendar';
import {Agenda} from './agenda';
import {TopNavBar} from '../navigation/top-nav-bar';
import {DaysCounters} from './days-counters/days-counters';
import {NavigationScreenProps} from 'react-navigation';

const navBar = new TopNavBar('');

export class CalendarScreenImpl extends Component<NavigationScreenProps> {
    public static navigationOptions = navBar.configurate();

    public render() {
        return <View style={styles.container}>
            <DaysCounters/>
            <Calendar navigation={this.props.navigation}/>
            <Agenda/>
        </View>;
    }
}
