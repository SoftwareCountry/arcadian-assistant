import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';
import { Days } from './days/days';

export class CalendarScreen extends Component {

    public render() {
        return <View>
            <Days />
        </View>;
    }
}