import React from 'react';
import { FlatList, Text, View} from 'react-native';
import { TopNavBar } from '../topNavBar/top-nav-bar';

const navBar =  new TopNavBar('News');
const dataList = [
{name: 'First news'},
{name: 'Second news'},
{name: 'Third news'},
{name: 'Fourth news'},
{name: 'Fifth news'},
{name: 'Sixth news'},
{name: 'Seventh news'},
{name: 'Ten'}
];

export class HomeNewsScreen extends React.Component {
    public static navigationOptions = navBar.configurate();

    public render() {
        return (
            <View>
                <FlatList data = {dataList} renderItem ={({item}) => <Text style={{padding: 10, fontSize: 18, height: 81, textAlign: 'center'}}>{item.name}</Text>} />
            </View>
        );
    }
}