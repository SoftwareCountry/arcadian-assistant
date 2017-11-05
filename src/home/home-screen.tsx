import React, { Component } from 'react';
import { View, Text, Button } from 'react-native';
import { NavigationScreenProps } from 'react-navigation';
import Icon from 'react-native-vector-icons/Ionicons';
import styles from '../layout/styles';

export class HomeScreen extends Component<NavigationScreenProps<{}>> {

    public render() {

        return <View style={styles.container}>
            <Button title='Sicklist' onPress={() =>  this.props.navigation.navigate('Sicklist')} />
            <Button title='Actions' onPress={() =>  this.props.navigation.navigate('Actions')} />
            <Button title='Daysoff' onPress={() =>  this.props.navigation.navigate('Daysoff')} />
            <Button title='Vacations' onPress={() =>  this.props.navigation.navigate('Vacations')} />
        </View>;
    }
}