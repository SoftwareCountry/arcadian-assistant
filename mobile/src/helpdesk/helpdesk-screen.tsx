import React, { Component } from 'react';
import { Animated, ScrollView, View, Text, Dimensions, Image } from 'react-native';
import styles from '../layout/styles';
import { PersonCardWithAvatar } from '../common';

const dataList = [
    { key: 'Ivan Ivanov' },
    { key: 'Petr Petrov' },
    { key: 'Sergey Sergeev' },
    { key: 'Maxim Maximov' }
];

export class HelpdeskScreen extends Component {

    public render() {
        return <View>
            <Animated.ScrollView horizontal >
            <PersonCardWithAvatar personName='Ivan' />
            <PersonCardWithAvatar personName='Petr' />
            <PersonCardWithAvatar personName='Sergey' />
            </Animated.ScrollView>
        </View>;
    }
}